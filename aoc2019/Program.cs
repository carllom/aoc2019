using System;
using System.Linq;

namespace Advent.Of.Code
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0 && int.TryParse(args[0], out var day))
            {
                // Run specific day
                ExecuteTask(day);
                return;
            }

            // Run all days
            day = 1;
            while (ExecuteTask(day++)) { }
        }

        private static bool ExecuteTask(int day)
        {
            var task = GetTask(day);
            if (task == null) return false;
            Console.WriteLine($"Running day {day}, Task 1..");
            task.First();
            Console.WriteLine($"Running day {day}, Task 2..");
            task.Second();
            Console.WriteLine();
            return true;
        }

        private static AbstractAocTask GetTask(int day)
        {
            return day switch
            {
                1 => new Day1.CounterUpper(),   // Day 1: "The Tyranny of the Rocket Equation"
                2 => new Day2.GravAssist(),     // Day 2: "1202 Program Alarm"
                3 => new Day3.Manhattan(),      // Day 3: "Crossed Wires"
                4 => new Day4.SecureContainer(),// Day 4: "Secure Container"
                5 => new Day5.Diagnostics(),    // Day 5: "Sunny with a Chance of Asteroids"
                6 => new Day6.OrbitMap(),       // Day 6: "Universal Orbit Map"
                7 => new Day7.Amplification(),  // Day 7: "Amplification Circuit"
                8 => new Day8.SpaceImage(),     // Day 8: "Space Image Format"
                9 => new Day9.SensorBoost(),    // Day 9: "Sensor Boost"
                10 => new Day10.Asteroids(),    // Day 10: "Monitoring Station"
                11 => new Day11.PainterRobot(), // Day 11: "Space Police"
                12 => new Day12.Gravity(),      // Day 12: "The N-Body Problem"
                13 => new Day13.Arcade(),       // Day 13: "Care Package"
                14 => new Day14.Crafting(),     // Day 14: "Space Stoichiometry"
                //X => new DayX.Tbd(),          // Day X: "TBD"
                _ => null
            };
        }
    }
}
