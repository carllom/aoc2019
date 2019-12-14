using Advent.Of.Code;
using System;
using System.Linq;

namespace Day12
{
    /// <summary>
    /// Slightly optimized, but not good
    /// </summary>
    internal class Gravity1 : AbstractAocTask
    {
        // Slightly improved, but not much quicker
        private void Step(Moon[] moons)
        {
            // For every moon..
            for (int i = 0; i < moons.Length; i++)
            {
                // ..apply gravity influence to the pair of moons following in the list..
                for (int j = i + 1; j < moons.Length; j++)
                {
                    var dx = Math.Clamp(moons[j].x - moons[i].x, -1, 1);
                    moons[i].vx += dx; moons[j].vx += -dx;
                    var dy = Math.Clamp(moons[j].y - moons[i].y, -1, 1);
                    moons[i].vy += dy; moons[j].vy += -dy;
                    var dz = Math.Clamp(moons[j].z - moons[i].z, -1, 1);
                    moons[i].vz += dz; moons[j].vz += -dz;
                }
            }
            // ..then apply the velocity
            for (int i = 0; i < moons.Length; i++)
            {
                moons[i].x += moons[i].vx;
                moons[i].y += moons[i].vy;
                moons[i].z += moons[i].vz;
            }
        }

        public void First()
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