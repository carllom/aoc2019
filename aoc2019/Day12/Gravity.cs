using Advent.Of.Code;
using System;

namespace Day12
{
    /// <summary>
    /// Component-wise solution. The way to go
    /// </summary>
    internal class Gravity : AbstractAocTask
    {
        /// <summary>
        /// Calculate per component
        /// </summary>
        /// <param name="moons">array [moon#,pos(0)|vel(1),component]</param>
        /// <param name="xyz">component x(0)|y(1)|z(2)</param>
        private void Step(int[,,] moons, int xyz)
        {
            // For every moon..
            for (int i = 0; i < 4; i++)
            {
                // ..apply gravity influence to the pair of moons following in the list..
                for (int j = i + 1; j < 4; j++)
                {
                    var dx = Math.Clamp(moons[j,0,xyz] - moons[i,0,xyz], -1, 1);
                    moons[i,1,xyz] += dx; moons[j,1,xyz] += -dx;
                }
            }
            // ..then apply the velocity
            for (int i = 0; i < 4; i++)
            {
                moons[i,0,xyz] += moons[i,1,xyz];
            }
        }

        public override void First()
        {
            var moons3 = new int[4, 2, 3]
            {
                { {14, 2, 8}, {0,0,0} },
                { { 7, 4,10}, {0,0,0} },
                { { 1,17,16}, {0,0,0} },
                { {-4,-1, 1}, {0,0,0} }
            };
            for (int i = 0; i < 1000; i++)
            {
                Step(moons3,0); // x
                Step(moons3,1); // y
                Step(moons3,2); // z
            }

            var totalEnergy = 0;
            for (int j =0;j<4;j++)
            {
                var pe = 0; // potential energy
                var ke = 0; // kinetic energy
                for(int k=0;k<3;k++) // iterate over xyz
                {
                    pe += Math.Abs(moons3[j, 0, k]);
                    ke += Math.Abs(moons3[j, 1, k]);
                }
                totalEnergy += pe * ke; // Total energy for moon is potential energy * kinetic energy
            }
            Echo($"Total energy after 1000 steps: {totalEnergy}");
            ValidateAnswer(totalEnergy, 9139);
        }

        public override void Second()
        {
            // third coordinate is start coordinate
            var moons3 = new int[4, 3, 3]
            {
                { {14, 2, 8}, {0,0,0}, {14, 2, 8} },
                { { 7, 4,10}, {0,0,0}, { 7, 4,10} },
                { { 1,17,16}, {0,0,0}, { 1,17,16} },
                { {-4,-1, 1}, {0,0,0}, {-4,-1, 1} }
            };

            // iterate over coordinates
            var repeats = new long[3];
            for (int c=0; c<3; c++)
            {
                Step(moons3,c);
                long step = 1;
                while (!Repeat(moons3,c))
                {
                    Step(moons3, c);
                    step++;
                }
                repeats[c] = step;
            }
            var totalRepeat = lcm(lcm(repeats[0], repeats[1]), repeats[2]);
            Echo($"Number of steps for universe repeat {totalRepeat}");
            ValidateAnswer(totalRepeat, 420788524631496L);
        }

        private long gcf(long a, long b)
        {
            while (b != 0)
            {
                var x = b;
                b = a % b;
                a = x;
            }
            return a;
        }

        private long lcm(long a, long b)
        {
            return (a * b / gcf(a, b));
        }

        private bool Repeat(int[,,] moons, int xyz)
        {
            for (int i = 0; i < 4; i++)
            {
                if (moons[i,0,xyz] != moons[i,2,xyz] || moons[i,1,xyz] != 0)
                    return false;
            }
            return true;
        }
    }
}