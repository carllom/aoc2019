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
                1 => new Day1.CounterUpper(), // Day 1: "The Tyranny of the Rocket Equation"
                2 => new Day2.GravAssist(),   // Day 2: "1202 Program Alarm"
                3 => new Day3.Manhattan(),    // Day 3: "Crossed Wires"
                4 => new Day4.SecureContainer(), // Day 4: "Secure Container"
                5 => new Day5.Diagnostics(),  // Day 5: "Sunny with a Chance of Asteroids"
                //X => new DayX.Tbd(),        // Day X: "TBD"
                _ => null
            };
        }
    }
}
