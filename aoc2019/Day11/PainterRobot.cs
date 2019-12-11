using Advent.Of.Code;
using Advent.Of.Code.IntCode;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day11
{
    internal class PainterRobot : AbstractAocTask
    {
        List<(int x, int y)> painted = new List<(int x, int y)>();
        List<(int x, int y)> visited = new List<(int x, int y)>();

        public override void First()
        {
            var icm = new IntCodeMachine();
            icm.Init("Day11/paint.ic");

            icm.Input.Write(0); // Start on a black panel

            PaintHull(icm);
            Echo($"# of painted cells: {visited.Count}");
            ValidateAnswer(visited.Count, 2016);
        }

        private void PaintHull(IntCodeMachine icm)
        {
            long dir = 0;
            bool readDir = false;
            (int x, int y) cur = (x: 0, y: 0);
            while (icm.Step())
            {
                while (icm.Output.CanRead)
                {
                    if (!readDir)
                    {
                        var color = icm.Output.Read();
                        //System.Diagnostics.Debug.WriteLine($"SET ({cur.x},{cur.y}) = {color}");
                        if (!visited.Any(p => p == cur)) visited.Add(cur);

                        if (!painted.Any(p => p == cur))
                        {
                            if (color == 1) painted.Add(cur);
                        }
                        else
                        {
                            if (color == 0) painted.Remove(cur);
                        }
                        readDir = true;
                    }
                    else
                    {
                        dir = (dir + 4 + (icm.Output.Read() == 1 ? 1 : -1)) % 4; // Rotate 0..3
                        switch (dir) // ..and move
                        {
                            case 0: // Up
                                cur.y++;
                                break;
                            case 1: // Right
                                cur.x++;
                                break;
                            case 2: // Down
                                cur.y--;
                                break;
                            case 3: // Left
                                cur.x--;
                                break;
                        }
                        icm.Input.Write(painted.Any(p => p == cur) ? 1 : 0); // On white or black?
                        readDir = false;
                    }
                }
            }
        }

        public override void Second()
        {
            var icm = new IntCodeMachine();
            icm.Init("Day11/paint.ic");
            painted.Clear();
            visited.Clear();

            icm.Input.Write(1); // Start on a white panel
            painted.Add((x: 0, y: 0)); // Starting white panel is painted

            PaintHull(icm);
            Console.WriteLine(); // Space above
            DisplayHull();
            Console.WriteLine(); // Space below
        }

        private void DisplayHull()
        {
            // Get paint boundaries
            int xmin = painted.Min(p => p.x), xmax = painted.Max(p => p.x);
            int ymin = painted.Min(p => p.y), ymax = painted.Max(p => p.y);
            for (int y = ymax; y >= ymin; y--) // positive y is up
            {
                for (var x = xmin; x <= xmax; x++)
                {
                    Console.Write(painted.Any(p => p == (x, y)) ? '#' : ' '); // Paint cell
                }
                Console.WriteLine(); // Next y
            }
        }
    }
}