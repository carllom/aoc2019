using Advent.Of.Code;
using Advent.Of.Code.IntCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Day13
{
    internal class Arcade : AbstractAocTask
    {
        public override void First()
        {
            var icm = new IntCodeMachine();
            icm.Init("Day13/breakout.ic");

            icm.Input.Write(0); // Start on a black panel

            Play(icm);
            var tilesLeft = tiles.Count(t => t.Value == 2);
            Echo($"# of tiles left: {tilesLeft}");
            ValidateAnswer(tilesLeft, 306);
        }

        Dictionary<(long x, long y),long> tiles = new Dictionary<(long x, long y), long>();

        private void Play(IntCodeMachine icm)
        {
            int phase = 0;
            (long x, long y) cur = (x: 0, y: 0);
            while (icm.Step())
            {
                while (icm.Output.CanRead)
                {
                    switch (phase)
                    {
                        case 0: // x pos
                            cur.x = icm.Output.Read();
                            phase = (phase + 1) % 3;
                            break;
                        case 1: // y pos
                            cur.y = icm.Output.Read();
                            phase = (phase + 1) % 3;
                            break;
                        case 2: // tile
                            tiles[cur] = icm.Output.Read();
                            phase = (phase + 1) % 3;
                            break;
                        default:
                            break;
                    }
                }
                if (icm.WantInput)
                {
                    RenderScreen(icm);
                }
            }
        }

        private char[] tilemap = new[] { ' ', '/', '#', '=', 'o' };
        private long RenderScreen(IntCodeMachine icm)
        {
            var score = 0L;
            Console.CursorLeft = 0;
            Console.CursorTop = 0;
            Console.WriteLine($"tiles: {tiles.Count(t => t.Value == 2)}     ");
            foreach (var row in tiles.GroupBy(t => t.Key.y, t => t).OrderBy(t => t.Key))
            {
                var rowSb = new StringBuilder(50);
                foreach (var item in row.OrderBy(t => t.Key.x))
                {
                    if (item.Key.x == -1)
                    {
                        Console.WriteLine($"SCORE: {item.Value}     ");
                        score = item.Value;
                    }
                    else
                        rowSb.Append(tilemap[item.Value]);
                }
                Console.WriteLine(rowSb.ToString());
            }
            CalcInput(icm);
            return score;
        }

        bool ai = true;

        private void CalcInput(IntCodeMachine icm)
        {
            // AI input
            if (ai)
            {
                var ballX = tiles.First(t => t.Value == 4).Key.x;
                var padX = tiles.First(t => t.Value == 3).Key.x;
                icm.Input.Write(Math.Clamp(ballX - padX, -1, 1));
                return;
            }

            // Meatbag input
            var key = Console.ReadKey();
            switch (key.KeyChar)
            {
                case 'z':
                case 'Z':
                    icm.Input.Write(-1); break;
                case 'x':
                case 'X':
                    icm.Input.Write(1); break;
                default: icm.Input.Write(0); break;
            }
        }

        public override void Second()
        {
            var icm = new IntCodeMachine();
            icm.Init("Day13/breakout.ic");

            icm.mem[0] = 2; // 2 quarters
            Play(icm);
            var score = RenderScreen(icm);
            Echo($"Final score: {score}");
            ValidateAnswer(score, 15328);
        }
    }
}