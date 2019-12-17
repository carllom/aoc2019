using Advent.Of.Code;
using Advent.Of.Code.IntCode;
using System;
using Utils;

namespace Day17
{
    internal class Scaffold : AbstractAocTask
    {
        public Scaffold()
        {
        }

        public override void First()
        {
            var icm = new IntCodeMachine();
            icm.Init("Day17/cameras.ic");
            var map = GenerateMap(icm);
            int val = FindIntersections(map);
            map.Render();
            Echo($"Alignment parameter sum: {val}");
            ValidateAnswer(val, 2080);
        }

        private SparseMap<char> GenerateMap(IntCodeMachine icm)
        {
            var map = new SparseMap<char>(true);
            (int x, int y) cur = (0, 0);


            while (icm.Step())
            {
                while (icm.Output.CanRead)
                {
                    var tile = icm.Output.Read();
                    //Console.Write((char)tile);
                    if (tile == 10)
                    {
                        cur.x=0;
                        cur.y++;
                    } else
                    {
                        map.Set(cur, (char)tile);
                        cur.x++;
                    }
                }
                if (icm.WantInput)
                {
                }
            }
            return map;
        }
        private int FindIntersections(SparseMap<char> map)
        {
            var alignment = 0;
            for (int y = map.Ymin; y < map.Ymax; y++)
            {
                for (int x = map.Xmin; x < map.Xmax; x++)
                {
                    var c = map.Get((x, y));
                    var neigh = 0;
                    if (c == '#')
                    {
                        neigh += map.Get((x - 1, y)) == c ? 1 : 0;
                        neigh += map.Get((x + 1, y)) == c ? 1 : 0;
                        neigh += map.Get((x, y - 1)) == c ? 1 : 0;
                        neigh += map.Get((x, y + 1)) == c ? 1 : 0;
                    }
                    if (neigh > 2)
                    {
                        map.Set((x, y), 'O');
                        alignment += x * y;
                    }
                }
            }
            return alignment;
        }

        public override void Second()
        {
            var icm = new IntCodeMachine();
            icm.Init("Day17/cameras.ic");
            icm.mem[0] = 2; // Override movement control
            var map = new SparseMap<char>(true);
            var y0 = Console.CursorTop;
            (int x, int y) cur = (0, 0);
            int cmd = 0, cIdx = 0;
            var dust = 0L;
            while (icm.Step())
            {
                while (icm.Output.CanRead)
                {
                    var tile = icm.Output.Read();
                    if (tile == 10)
                    {
                        cur.x = 0;
                        cur.y++;
                    }
                    else if (tile < 256)
                    {
                        map.Set(cur, (char)tile);
                        cur.x++;
                    }
                    else
                    {
                        dust = tile;
                    }
                }
                if (icm.WantInput)
                {
                    if (cmd < movements.Length && cIdx < movements[cmd].Length)
                    {
                        icm.Input.Write((long)movements[cmd][cIdx]);
                        cIdx++;
                    } 
                    else
                    {
                        cmd++;
                        cIdx = 0;
                    }
                }
            }
            map.Render();
            Echo($"Dust collected: {dust}");
            ValidateAnswer(dust, 742673);
        }

        string [] movements = {
            //12345678901234567890
             "A,B,B,A,C,A,C,A,C,B\n", // Main
             "L,6,R,12,R,8\n", // A 
             "R,8,R,12,L,12\n", // B
             "R,12,L,12,L,4,L,4\n", // C
             "n\n" // Continuous video feed
            };

        //L,6,R,12,R,8,R,8,R,12,L,12,R,8,R,12,L,12,L,6,R,12,R,8,R,12,L,12,L,4,L,4,L,6,R,12,R,8,R,12,L,12,L,4,L,4,L,6,R,12,R,8,R,12,L,12,L,4,L,4,R,8,R,12,L,12
        //aaaaaaaaaaaa bbbbbbbbbbbbb bbbbbbbbbbbbb aaaaaaaaaaaa ccccccccccccccccc aaaaaaaaaaaa ccccccccccccccccc aaaaaaaaaaaa ccccccccccccccccc bbbbbbbbbbbbb
    }
}