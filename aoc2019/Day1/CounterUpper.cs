using System;
using System.IO;

namespace Advent.Of.Code.Day1
{
    class CounterUpper : AbstractAocTask
    {
        public override void First()
        {
            using (var data = new StreamReader("Day1/rocketmodules.txt"))
            {
                long totalFuel = 0;
                while (!data.EndOfStream)
                {
                    if (int.TryParse(data.ReadLine(), out var module))
                    {
                        totalFuel += Math.Max(module / 3 - 2, 0);
                    }
                }
                Echo($"Total fuel demand for modules: {totalFuel}");
                ValidateAnswer(totalFuel, 3239890);
            }
        }

        public override void Second()
        {
            using (var data = new StreamReader("Day1/rocketmodules.txt"))
            {
                long totalFuel = 0;
                while (!data.EndOfStream)
                {
                    if (int.TryParse(data.ReadLine(), out var module))
                    {
                        var requiredFuel = module / 3 - 2;
                        while (requiredFuel > 0)
                        {
                            totalFuel += requiredFuel;
                            requiredFuel = requiredFuel / 3 - 2;
                        }
                    }
                }
                Echo($"Total fuel demand (modules+fuel): {totalFuel}");
                ValidateAnswer(totalFuel, 4856963);
            }
        }
    }
}
