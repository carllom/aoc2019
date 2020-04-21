using Advent.Of.Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Day18
{
    internal class Tunnels : AbstractAocTask
    {
        private TunnelMap map;
        public override void First()
        {
            map = new TunnelMap("Day18/tunnels.txt");
            //var keys = map.FindKeys();
            //var reach = keys.ToDictionary(k => k.Key, k => map.ReachFrom(k.Value));

            var totalDist = 0;
            map.debugplot = PlotMode.None;
            var origin = map.Find('@');
            map.Set(origin, '.');
            var bestof = DoReach(origin);
        }

        long keysTried = 0;

        private (int dist, string keys) DoReach((int x, int y) currPos, string keys = "", int dist = 0, int shortestYet = int.MaxValue, string forKeys ="")
        {
            if (keys.Length == 26) return (dist, keys); // We have all keys
            var r = map.ReachFrom(currPos, keys).OrderBy(a => a.dist); // prioritize short distances
            // iterate over keys within reach - branch for each key 
            foreach (var item in r)
            {
                if (IsKey(item.c) && !keys.Contains(item.c)) // Is it a new key? 
                {
                    if (dist + item.dist > shortestYet)
                        continue; // Dont bother continuing if we already have a better suggestion
                    var res = DoReach(item.pos, keys + item.c, dist + item.dist, shortestYet, forKeys);
                    if (res.dist < shortestYet)
                    {
                        Console.WriteLine($"{res.keys} => {res.dist} is shortest path! Currently evaluating {keys + item.c}");
                        shortestYet = res.dist; forKeys = res.keys; // We have a new shortest path
                    }
                }
            }
            return (shortestYet, forKeys); // get the shortest distance
        }
    /*
        private (int dist, string keys) DoReach((int x, int y) currPos, string keys = "", int dist = 0, int shortestYet = int.MaxValue, string forKeys = "")
        {
            if (keys.Length == 26) return (dist, keys);
            //if (keys.Length == 26) return new[] { (dist, keys) };
            //var result = new List<(int dist, string keys)>();
            var r = map.ReachFrom(currPos, keys).OrderBy(a => a.dist); // prioritize short distances
            // iterate over keys within reach - branch for each key 
            foreach (var item in r)
            {
                if (IsKey(item.c) && !keys.Contains(item.c)) // Is it a new key? 
                {
                    Console.Write($"{keys + item.c} {dist + item.dist}");
                    var res = DoReach(item.pos, keys + item.c, dist + item.dist));
                    if (res.dist < shortestYet)
                    {
                        shortestYet = res.dist; forKeys = res.keys;
                    }
                    //result.AddRange(DoReach(item.pos, keys + item.c, dist + item.dist));
                }
            }
            return result.OrderBy(r => r.dist).Take(1); // get the shortest distance
        }
        */
        private bool IsKey(char c) => char.IsLower(c);
        private bool IsDoor(char c) => char.IsUpper(c);

        public override void Second()
        {
            base.Second();
        }
    }

    enum PlotMode
    {
        None,
        Final,
        Step
    }

    class TunnelMap
    {
        private string path;
        char[,] map;
        char[,] _copy;
        internal PlotMode debugplot;

        public TunnelMap(string path)
        {
            this.path = path;
            map = ParseMap(path);
        }

        public void Reset()
        {
            map = ParseMap(path);
        }

        private void PushMap()
        {
            _copy = map.Clone() as char[,];
        }

        private void PopMap()
        {
            var tmp = map;
            map = _copy;
            _copy = map;
        }

        private static char[,] ParseMap(string path)
        {
            var data = File.ReadAllLines(path);
            var map = new char[data.Max(r => r.Length), data.Length];
            var y = 0;
            foreach (var row in data)
            {
                for (int x = 0; x < row.Length; x++)
                {
                    map[x, y] = row[x];
                }
                y++;
            }
            return map;
        }

        public (int x, int y) Find(char c)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    if (map[x, y] == c) return (x, y);
                }
            }
            return default;
        }

        public bool Key((int x, int y) pos) => At(pos) >= 'a' && At(pos) <= 'z'; //letters.ToLower().Contains();
        private bool Door((int x, int y) pos) => At(pos) >= 'A' && At(pos) <= 'Z'; //letters.ToUpper().Contains(At(pos));
        private bool Dude((int x, int y) pos) => At(pos) == '@';
        private  char At((int x, int y) pos) => map[pos.x, pos.y];
        public void Set((int x, int y) pos, char c) => map[pos.x, pos.y] = c;
        public IDictionary<char, (int x, int y)> FindKeys()
        {
            var keys = new Dictionary<char, (int x, int y)>();
            foreach (var key in "abcdefghijklmnopqrstuvwxyz")
            {
                var keyPos = Find(key);
                if (keyPos != default)
                {
                    keys[key] = keyPos;
                }
            }
            return keys;
        }

        public IDictionary<char, (int x, int y)> FindDoors()
        {
            var doors = new Dictionary<char, (int x, int y)>();
            foreach (var door in "ABCDEFGHIJKLMNOPQRSTUVXYZ")
            {
                var doorPos = Find(door);
                if (doorPos != default)
                {
                    doors[door] = doorPos;
                }
            }
            return doors;
        }

        public IEnumerable<ReachResult> ReachFrom((int x, int y) pos, string keys = "")
        {
            PushMap();
            var result = LocateAny(pos, pos, keys);
            if (debugplot == PlotMode.Final) DebugPlot();
            PopMap();
            return result.Where(r => r != null);
        }

        public int OpenDist(char from, char to)
        {
            var fromPos = Find(from);
            var toPos = Find(to);
            var minDist = OpenDist(fromPos, fromPos, toPos, 0).Min();
            if (debugplot == PlotMode.Final) DebugPlot();
            Reset(); // Restore path marks
            return minDist;
        }

        private IEnumerable<int> OpenDist((int x, int y) pos, (int x, int y) prev, (int x, int y) to, int dist)
        {
            map[pos.x, pos.y] = ' '; // Mark as visited
            if (debugplot == PlotMode.Step) DebugPlot();
            List<int> found = new List<int>();

            foreach (var neighbor in pos.Neighbors().Where(p => p != prev))
            {
                if (to == neighbor) found.Add(dist + 1);
                if (CanWalk(neighbor))
                    found.AddRange(OpenDist(neighbor, pos, to, dist + 1));
            }
            return found;
        }

        IEnumerable<ReachResult> LocateAny((int x, int y) pos, (int x, int y) prev, string keys, int dist = 0)
        {
            map[pos.x, pos.y] = ' '; // Mark as visited
            if (debugplot == PlotMode.Step) DebugPlot();
            var found = new ReachResult[26*2];
            var fIdx = 0;
            //List<((int x, int y) pos, char c, int dist)> found = new List<((int x, int y) pos, char c, int dist)>();
            
            foreach (var neighbor in new[] {(pos.x +1,pos.y),(pos.x-1,pos.y), (pos.x, pos.y +1), (pos.x, pos.y -1) })
            {
                if (neighbor == prev) continue;
                if (Door(neighbor) || Dude(neighbor) || Key(neighbor)) found[fIdx++] = new ReachResult(neighbor, At(neighbor), dist + 1);
                if (CanWalk(neighbor, keys))
                {
                    foreach (var item in LocateAny(neighbor, pos, keys, dist + 1))
                    {
                        if (item == default) break;
                        found[fIdx++] = item;
                    }
                }
            }
            return found;
        }

        private void DebugPlot()
        {
            var y0 = Console.CursorTop;
            Plot();
            Console.CursorTop = y0;
        }

        bool CanWalk((int x, int y) pos, string keys = "")
        {
            var c = map[pos.x, pos.y];
            //if (keys.Contains(char.ToLower(c)) || (keys.Length > 0 && char.IsUpper(c)))
            //    Console.Write("!");
            return c == '.' || (c >= 'a' && c <= 'z') || keys.Contains(char.ToLower(c)); // tunnel or key or door that we have key for
        }

        IEnumerable<(int x, int y)> Walkable((int x, int y) pos) => pos.Neighbors().Where(n => CanWalk(n));

        public void Plot()
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                var sb = new StringBuilder(map.GetLength(0));
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    sb.Append(map[x, y]);
                }
                Console.WriteLine(sb.ToString());
            }
        }
    }

    public sealed class ReachResult
    {
        public (int x, int y) pos;
        public char c;
        public int dist;

        public ReachResult((int x, int y) pos, char c, int dist)
        {
            this.pos = pos; this.c = c; this.dist = dist;
        }
    }

    public static class CoordinateExtensions 
    {
        public static (int x, int y) Up(this (int x, int y) pos) => (pos.x, pos.y - 1);
        public static (int x, int y) Down(this (int x, int y) pos) => (pos.x, pos.y + 1);
        public static (int x, int y) Left(this (int x, int y) pos) => (pos.x - 1, pos.y);
        public static (int x, int y) Right(this (int x, int y) pos) => (pos.x + 1, pos.y);
        public static (int x, int y)[] Neighbors(this (int x, int y) pos) => new[] { pos.Up(), pos.Down(), pos.Left(), pos.Right() };
    }
}