using Advent.Of.Code;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day6
{
    class OrbitMap : AbstractAocTask
    {

        Dictionary<string, Orbit> map = new Dictionary<string, Orbit>();

        public OrbitMap()
        {
            using (var data = new StreamReader("Day6/orbits.txt"))
            {
                while (!data.EndOfStream)
                {
                    var entry = data.ReadLine().Split(')');
                    if (entry.Length != 2) continue;
                    Orbit.map.Add(entry[1], new Orbit(entry[1], entry[0]));
                }
            }
        }

        public override void First()
        {

            foreach (var orbit in Orbit.map.Values)
            {
                var p = orbit;
                while (Orbit.map.ContainsKey(p.parentName))
                {
                    orbit.depth++;
                    p = p.parent;
                }
            }
            var numOrbits = Orbit.map.Values.Sum(o => o.depth);
            Echo($"The sum of direct and indirect orbits is {numOrbits}");
            ValidateAnswer(numOrbits, 223251);
        }
        public override void Second()
        {
            var san = Orbit.map["SAN"];
            var you = Orbit.map["YOU"];
            var dist = san.depth + you.depth - 2; // First parent segment does not count for either leaf

            foreach (var p in you.parents)
            {
                if (san.parents.Contains(p))
                {
                    dist -= p.depth * 2;
                    break;
                }
            }
            Echo($"Distance YOU<->SAN : {dist}");
            ValidateAnswer(dist, 430);
        }
    }

    class Orbit
    {
        public static Dictionary<string, Orbit> map = new Dictionary<string, Orbit>();

        public int depth = 1;
        public string name;
        public string parentName;
        public Orbit parent => map.ContainsKey(parentName) ? map[parentName] : null;
        public Orbit(string name, string parent)
        {
            this.name = name;
            parentName = parent;
        }

        public IEnumerable<Orbit> parents
        {
            get
            {
                var parents = new List<Orbit>();
                var p = this.parent;
                while (p != null) { parents.Add(p); p = p.parent; }
                return parents;
            }
        }
    }
}