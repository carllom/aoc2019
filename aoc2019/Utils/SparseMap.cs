using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils
{
    class SparseMap<T>
    {
        private bool flipY;
        public Dictionary<(int x, int y), T> map = new Dictionary<(int x, int y), T>();

        public int Xmin => map.Min(p => p.Key.x);
        public int Xmax => map.Max(p => p.Key.x);
        public int Ymin => map.Min(p => p.Key.y);
        public int Ymax => map.Max(p => p.Key.y); 

        public SparseMap(bool flipY = false)
        {
            this.flipY = flipY;
        }

        public T Get((int x, int y) coord)
        {
            return map.ContainsKey(coord) ? map[coord] : default;
        }

        public void Set((int x, int y) coord, T value)
        {
            map[coord] = value;
        }

        public void Render((int x, int y)? cur = null)
        {
            if (!map.Any()) return; // Empty map

            // Get paint boundaries
            int xmin = map.Min(p => p.Key.x), xmax = map.Max(p => p.Key.x);
            int ymin = map.Min(p => p.Key.y), ymax = map.Max(p => p.Key.y);
            for (int y0 = ymax; y0 >= ymin; y0--) // positive y is up
            {
                var y = flipY ? ymax - y0 : y0;
                var sb = new StringBuilder(xmax - xmin);
                for (var x = xmin; x <= xmax; x++)
                {
                    if (cur.HasValue && x == cur.Value.x && y == cur.Value.y)
                    {
                        sb.Append('D');
                    }
                    else if (map.ContainsKey((x, y)))
                    {
                        sb.Append(map[(x, y)]);
                    }
                    else
                    {
                        sb.Append(' ');
                    }
                }
                Console.WriteLine(sb.ToString());
            }
        }
    }
}