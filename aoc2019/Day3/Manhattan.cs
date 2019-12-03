using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace Advent.Of.Code.Day3
{
    class Manhattan : AbstractAocTask
    {
        public override void First()
        {
            var minManhattan = ParseWires().Min(i => i.intersection.Manhattan());
            Echo($"Min intersection manhattan distance: {minManhattan}");
            ValidateAnswer(minManhattan, 865);
        }

        public override void Second()
        {
            var minWireDistance = ParseWires().Min(i => i.WireDistance);
            Echo($"Intersection with min wire distance: {minWireDistance}");
            ValidateAnswer(minWireDistance, 35038);
        }

        private List<Intersection> ParseWires()
        {
            List<Intersection> intersections = new List<Intersection>();
            using (var wires = new StreamReader("Day3/wires.txt"))
            {
                var wireId = 0;
                while (!wires.EndOfStream)
                {
                    intersections.AddRange(ParseWire(wireId++, wires.ReadLine().Split(',')));
                }
            }
            return intersections;
        }

        private List<WireSegment> segments = new List<WireSegment>();

        private IEnumerable<Intersection> ParseWire(int id, string[] wireSegments)
        {
            List<Intersection> intersections = new List<Intersection>();
            Point from = Point.Origin; // Start @(0,0)
            var wireDistance = 0;
            foreach (var segment in wireSegments)
            {
                var length = int.Parse(segment.Substring(1));
                var to = (segment[0]) switch
                {
                    'U' => new Point(from.x, from.y + length),
                    'D' => new Point(from.x, from.y - length),
                    'L' => new Point(from.x - length, from.y),
                    'R' => new Point(from.x + length, from.y),
                    _ => throw new ArgumentOutOfRangeException(nameof(segment)),
                };
                var seg = new WireSegment(id, from, to, wireDistance);
                wireDistance += length; // Add segment length to wire distance
                intersections.AddRange(segments
                    .Where(s => s.wire != seg.wire && s.Intersects(seg)) // The same wire cannot intersect itself
                    .Select(s => new Intersection(s, seg, s.IntersectsAt(seg))));
                segments.Add(seg);
                from = to;
            }
            return intersections;
        }
    }

    class Point : IComparable
    {
        public static readonly Point Origin = new Point(0, 0);
        public readonly int x;
        public readonly int y;
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int Manhattan(Point other) => Math.Abs(x-other.x) + Math.Abs(y-other.y);

        public int Manhattan() => Math.Abs(x) + Math.Abs(y);

        public int CompareTo(object obj)
        {
            if (!(obj is Point other)) throw new ArgumentException($"parameter obj must be of type Point");
            return Manhattan() - other.Manhattan(); // Compare manhattan distances
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point other)) return false;
            return x == other.x && y == other.y;
        }
    }

    class WireSegment
    {
        public int distance;
        public int wire;
        public Point from;
        public Point to;

        public int Left => Math.Min(from.x, to.x);
        public int Right => Math.Max(from.x, to.x);
        public int Top => Math.Max(from.y, to.y);
        public int Bottom => Math.Min(from.y, to.y);

        public WireSegment(int wire, Point from, Point to, int distance)
        {
            this.wire = wire;
            this.from = from;
            this.to = to;
            this.distance = distance;
        }

        public bool Intersects(WireSegment other)
        {
            if (from.Equals(Point.Origin) && other.from.Equals(Point.Origin))
                return false; // Do not count origin
            return !(Right < other.Left || Left > other.Right || Bottom > other.Top || Top < other.Bottom);
        }

        public Point IntersectsAt(WireSegment other)
        {
            var y = from.y == to.y ? from.y : other.from.y; // use y from horizontal segment
            var x = from.x == to.x ? from.x : other.from.x; // use x from vertical segment
            return new Point(x, y);
        }
    }

    class Intersection
    {
        public readonly WireSegment seg1;
        public readonly WireSegment seg2;
        public readonly Point intersection;
        public Intersection(WireSegment seg1, WireSegment seg2, Point intersection)
        {
            this.seg1 = seg1;
            this.seg2 = seg2;
            this.intersection = intersection;
        }

        // distance to start of segment + distance to intersection for both segment 1 and 2
        public int WireDistance => seg1.distance + intersection.Manhattan(seg1.from) + seg2.distance + intersection.Manhattan(seg2.from);
    }
}
