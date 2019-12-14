using Advent.Of.Code;
using System;
using System.Linq;

namespace Day12
{
    // The first naive implementation
    internal class Gravity0 : AbstractAocTask
    {
        private void Step(Moon[] moons)
        {
            // For every moon..
            foreach (var moon in moons)
            {
                // ..apply gravity influence from all other moons..
                foreach (var other in moons.Where(m => m != moon))
                {
                    moon.UpdateVel(other);
                }
            }
            // ..then apply the velocity
            foreach (var moon in moons)
            {
                moon.ApplyVel();
            }
        }

        public override void First()
        {
            Moon[] moons = new Moon[] {
                new Moon(14, 2, 8),
                new Moon(7, 4, 10),
                new Moon(1, 17, 16),
                new Moon(-4, -1, 1)
            };

            for (int i = 0; i < 1000; i++)
            {
                Step(moons);
            }
            var totalEnergy = moons.Sum(m => m.PotentialEnergy * m.KineticEnergy);
            Echo($"Total energy after 1000 steps: {totalEnergy}");
            ValidateAnswer(totalEnergy, 9139);
        }

        // runs forever..
        public void Second()
        {
            Moon[] moons = new Moon[] {
                new Moon(14, 2, 8),
                new Moon(7, 4, 10),
                new Moon(1, 17, 16),
                new Moon(-4, -1, 1)
            };
            Step(moons);
            var step = 1L;
            while (!Repeat(moons))
            //while (!moons.All(m => m.Repeat))
            {
                Step(moons);
                step++;
                if (step % 1000000 == 0) Console.Write('.');
            }
            Echo($"Number of steps {step}");
        }

        private bool Repeat(Moon[] moons)
        {
            for (int i = 0; i < moons.Length; i++)
            {
                if (moons[i].x != moons[i].x0 || moons[i].y != moons[i].y0 || moons[i].z != moons[i].z0)
                    return false;
            }
            return true;
        }
    }
}