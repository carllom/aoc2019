using Advent.Of.Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day10
{
    internal class Asteroids : AbstractAocTask
    {
        private List<Point> asteroids = new List<Point>();
        private Point astBase;

        public override void First() {
            using (var astmap = new StreamReader("Day10/astermap.txt")) {
                var y = 0;
                while (!astmap.EndOfStream) {
                    var l = astmap.ReadLine();
                    for (int x = 0; x < l.Length; x++) {
                        if (l[x] == '#') asteroids.Add(new Point(x, y));
                    }
                    y++;
                }
            }

            Point maxast = null;
            int numvisible = 0;

            foreach (var a0 in asteroids) {
                List<Point> visible = new List<Point>();
                foreach (var a1 in asteroids) {
                    if (a0 == a1) continue;
                    if (a0.y == a1.y) // Same horizontal
                    {
                        if (asteroids.Any(a => a.y == a0.y && Math.Max(a0.x, a1.x) > a.x && Math.Min(a0.x, a1.x) < a.x))
                            continue; // Something in between
                        else
                            visible.Add(a1); continue;
                    }
                    if (a0.x == a1.x) // Same vertical
                    {
                        if (asteroids.Any(a => a.x == a0.x && Math.Max(a0.y, a1.y) > a.y && Math.Min(a0.y, a1.y) < a.y))
                            continue; // Something in between
                        else
                            visible.Add(a1); continue;
                    }
                    if (Math.Abs(a0.x - a1.x) == 1 || Math.Abs(a0.y - a1.y) == 1) // Distance of 1 in one of the axes is always visible
                    {
                        visible.Add(a1); continue;
                    }
                    var q = gcf(Math.Abs(a1.x - a0.x), Math.Abs(a1.y - a0.y));
                    if (q == 1) // No common divisor - clear path
                    {
                        visible.Add(a1); continue;
                    }
                    else
                    {
                        int xs = (a1.x - a0.x) / q, ys = (a1.y - a0.y) / q;
                        int x = a0.x + xs, y = a0.y + ys;
                        while (x != a1.x && y != a1.y)
                        {
                            if (asteroids.Any(a => a.x == x && a.y == y))
                            {
                                break;
                            }
                            x += xs; y += ys;
                        }
                        if (x == a1.x && y == a1.y)
                        {
                            visible.Add(a1); continue;
                        }
                    }
                }
                if (visible.Count > numvisible)
                {
                    numvisible = visible.Count;
                    maxast = a0;
                }
            }
            astBase = maxast;
            Echo($"Asteroid with max # visible is at x={maxast.x}, y={maxast.y} with {numvisible}/{asteroids.Count()} asteroids visible");
            ValidateAnswer(numvisible, 214);
        }

        private int gcf(int a, int b)
        {
            while (b != 0)
            {
                int x = b;
                b = a % b;
                a = x;
            }
            return a;
        }

        public override void Second() {
            asteroids.Remove(astBase);
            foreach (var ast in asteroids)
            {
                ast.baseAng = astBase.AngleTo(ast);
                ast.baseDist = astBase.Dist(ast);
            }

            var clockwise = asteroids.GroupBy(a => a.baseAng).OrderBy(a => a.Key).Skip(199).First().First();
            Echo($"The 200th asteroid is at x={clockwise.x}, y={clockwise.y}");
            ValidateAnswer(clockwise.x * 100 + clockwise.y, 502);
        }
    }

    class Point {
        public double baseAng;
        public double baseDist;
        public readonly int x;
        public readonly int y;

        public Point(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public double Dist(Point other) => Math.Sqrt((other.x - x) * (other.x - x) + (other.y - y) * (other.y - y));

        public double AngleTo(Point other) {
            var ang = Math.Atan2(other.x - x, y - other.y);
            if (ang < 0) ang += 2 * Math.PI;
            return 180 * ang / Math.PI;
        }
    }
}