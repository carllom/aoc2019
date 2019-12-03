using System;

namespace Advent.Of.Code
{
    /// <summary>
    /// ***REMOVED***
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            if (!int.TryParse(args[0], out var day))
            {
                Console.WriteLine("Requires day (number)");
                return;
            }

            var task = GetTask(day);
            Console.WriteLine($"Running day {day}, Task 1..");
            task.First();
            Console.WriteLine($"Running day {day}, Task 2..");
            task.Second();
        }

        private static AbstractAocTask GetTask(int day)
        {
            switch(day)
            {
                case 1: return new Day1.CounterUpper();
                case 2: return new Day2.GravAssist();
                case 3: return new Day3.Manhattan();
                default: throw new NotImplementedException($"Day {day} is not implemented");
            }
        }
    }
}
