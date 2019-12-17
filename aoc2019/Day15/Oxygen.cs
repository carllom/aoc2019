using Advent.Of.Code;
using Advent.Of.Code.IntCode;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Day15
{
    internal class Oxygen : AbstractAocTask
    {
        SparseMap<char> map;

        public override void First()
        {
            var icm = new IntCodeMachine();
            icm.Init("Day15/repairdroid.ic");
            map = CreateMap(icm);
            int val = ShortestPath(map);
            Echo($"Shortest path from (0,0) to oxygen tank in {val} steps");
            ValidateAnswer(val, 218);
        }

        private (int x, int y)[] directions =
        {
            ( 0, 1), // N
            ( 1, 0), // E
            ( 0,-1), // S
            (-1, 0), // W
        };

        private int[] dirCommand = { 1, 4, 2, 3 }; // N E S W

        private (int x, int y) Next((int x, int y) current, int dir)
        {
            return (x: current.x + directions[dir].x, y: current.y + directions[dir].y);
        }

        private SparseMap<char> CreateMap(IntCodeMachine icm)
        {
            var y0 = Console.CursorTop;
            var map = new SparseMap<char>(true);
            bool wall = false, oxyfound = false;
            (int x, int y) cur = (0, 0), oxygen = (0, 0);
            map.Set(cur, '.'); // Start @ free position
            int dir = 0;

            while (icm.Step())
            {
                while (icm.Output.CanRead)
                {
                    var status = icm.Output.Read();

                    switch (status)
                    {
                        case 0: // wall
                            map.Set(Next(cur, dir), '#'); // Map wall
                            wall = true;
                            break;
                        case 1: // move
                            cur = Next(cur, dir);
                            wall = false;
                            map.Set(cur, '.');
                            break;
                        case 2: // oxygen
                            cur = Next(cur, dir);
                            oxygen = cur;
                            oxyfound = true;
                            wall = false;
                            map.Set(cur, '%');
                            break;
                        default:
                            break;
                    }
                }
                if (icm.WantInput)
                {
                    if (wall)
                        dir = (dir + 1) % 4; // Turn right 
                    else
                        dir = (dir + 3) % 4; // Turn left
                    icm.Input.Write(dirCommand[dir]);
                    Console.CursorLeft = 0;
                    Console.CursorTop = y0;
                    Console.WriteLine($"Cur: {cur.x},{cur.y}, dir {"NESW"[dir]}       ");
                    map.Render(cur);
                    if (oxyfound && cur == (0, 0))
                        break;
                }
            }
            return map;
        }

        private int ShortestPath(SparseMap<char> map)
        {
            return Traverse(map, (0, 0), (0, 0), 0);
        }

        private int Traverse(SparseMap<char> map, (int x, int y) pos, (int x, int y) prev, int count)
        {
            List<int> branches = new List<int>();
            for (int d = 0; d < 4; d++)
            {
                var next = Next(pos, d);
                if (next == prev) continue; // Skip where we came from 
                
                switch (map.Get(next))
                {
                    case '.': // Valid path
                        branches.Add(Traverse(map, next, pos, count + 1));
                        break;
                    case '#': // Wall
                        branches.Add(int.MaxValue);
                        break;
                    case '%': // Oxygen - terminal position
                        branches.Add(count + 1);
                        break;
                }
            }
            return branches.Min();
        }

        public override void Second()
        {
            var y0 = Console.CursorTop;
            var minutes = 0;
            var oxytank = map.map.First(c => c.Value == '%');
            map.Set(oxytank.Key, 'O'); // Initially the tank is filled with oxygen
            while (map.map.Any(c => c.Value == '.')) // Still places without oxygen
            {
                foreach (var coord in map.map.Where(c => c.Value == 'O').Select(c => c.Key).ToArray())
                {
                    for (int d = 0; d < 4; d++)
                    {
                        var next = Next(coord, d);
                        if (map.Get(next) == '.')
                        {
                            map.Set(next, 'O');
                        }
                    }
                }
                minutes++;
                Console.CursorLeft = 0;
                Console.CursorTop = y0;
                map.Render();
            }

            Echo($"Oxygen levels restored in {minutes} minutes");
            ValidateAnswer(minutes, 544);
        }
    }
}