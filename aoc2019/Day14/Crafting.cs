using Advent.Of.Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day14
{
    internal class Crafting : AbstractAocTask
    {
        private Dictionary<string, Substance> substances = new Dictionary<string, Substance>();
        private Substance GetSubstance(string name)
        {
            if (!substances.ContainsKey(name))
                substances[name] = new Substance(name);
            return substances[name];
        }

        private void ParseCraftGraph(string path)
        {
            var recipeRx = new Regex(@"(?<amt>\d+) (?<sub>\w+)");
            
            using (var sr = new StreamReader(path))
            {
                while(!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    var matches = recipeRx.Matches(line);
                    if (matches.Count < 2) throw new FormatException($"Illegal recipe format: {line}");

                    // Result substance is last match
                    var resMatch = matches.Last();
                    var resSub = GetSubstance(resMatch.Groups["sub"].Value);
                    var resAmount = int.Parse(resMatch.Groups["amt"].Value);

                    // Get ingredients (every match but last)
                    var recipe = new Dictionary<Substance, Tuple<int, int>>();
                    foreach (var ingMatch in matches.Reverse().Skip(1))
                    {
                        var ingSub = GetSubstance(ingMatch.Groups["sub"].Value);
                        var ingAmount = int.Parse(ingMatch.Groups["amt"].Value);

                        recipe[ingSub] = new Tuple<int, int>(ingAmount, resAmount);
                    }
                    resSub.recipes.Add(recipe);
                }
            }
        }

        public override void First()
        {
            ParseCraftGraph("Day14/crafting.txt");
            var requirements = CalculateRequirements(substances["FUEL"], 1);
            var numOre = requirements[substances["ORE"]];
            Echo($"Total ore quantity needed: {numOre}");
            ValidateAnswer(numOre, 1920219);
        }

        private bool isReducableTo(Substance from, Substance to)
        {
            if (from == to) return true;
            return from.recipes.Any(r => r.Keys.Any(i => isReducableTo(i,to)));
        }

        private Dictionary<Substance, long> CalculateRequirements(Substance s, long amount)
        {
            var requirements = new Dictionary<Substance, long>();
            requirements[s] = amount;

            // While we still have reduceable substances (substance with recipes)
            while (requirements.Any(r => r.Key.recipes.Any()))
            {
                // Grab the first reducable substance that no other current requirement is reduceable to
                var sub = requirements.First(r => r.Key.recipes.Any() && !requirements.Any(r2 => r2.Key != r.Key && isReducableTo(r2.Key, r.Key)));
                foreach (var ing in sub.Key.recipes.First())
                {
                    // Calculate required amount
                    var req = (long)Math.Ceiling((double)sub.Value / ing.Value.Item2) * ing.Value.Item1;
                    if (requirements.ContainsKey(ing.Key))
                    {
                        requirements[ing.Key] += req; // Add required amount for current substance
                    } 
                    else
                    {
                        requirements[ing.Key] = req;
                    }
                }
                requirements.Remove(sub.Key);
            }
            return requirements;
        }

        public override void Second()
        {
            ParseCraftGraph("Day14/crafting.txt");
            long req = 0;
            int fuel = 0;
            int inc = 1000000;
            while (inc > 0)
            {
                req = 0;
                while (req < 1000000000000L)
                {
                    fuel += inc;
                    var requirements = CalculateRequirements(substances["FUEL"], fuel);
                    req = requirements[substances["ORE"]];
                }
                fuel -= inc;
                inc /= 10;
            }
            req = CalculateRequirements(substances["FUEL"], fuel)[substances["ORE"]];
            Echo($"A trillion ORE gets you {fuel} FUEL (requires {req} ORE)");
            ValidateAnswer(fuel, 1330066);
        }
    }

    internal class Substance
    {
        public readonly string name;

        // A recipe is a list of substances with a normalized fractional number (the tuple) to produce 1 result
        public List<Dictionary<Substance, Tuple<int,int>>> recipes = new List<Dictionary<Substance, Tuple<int, int>>>();

        public Substance(string name)
        {
            this.name = name;
        }
    }
}