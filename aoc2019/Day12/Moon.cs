using System;

namespace Day12
{
    internal class Moon
    {
        public int x0;
        public int y0;
        public int z0;
        public int x;
        public int y;
        public int z;
        public int vx;
        public int vy;
        public int vz;

        public Moon(int x, int y, int z)
        {
            x0 = this.x = x; y0 = this.y = y; z0 = this.z = z;
            vx = vy = vz = 0;
        }

        public void UpdateVel(Moon other)
        {
            if (other.x > x) vx++; else if (other.x < x) vx--;
            if (other.y > y) vy++; else if (other.y < y) vy--;
            if (other.z > z) vz++; else if (other.z < z) vz--;
        }

        public void ApplyVel()
        {
            x += vx;
            y += vy;
            z += vz;
        }

        public int PotentialEnergy => Math.Abs(x) + Math.Abs(y) + Math.Abs(z);
        public int KineticEnergy => Math.Abs(vx) + Math.Abs(vy) + Math.Abs(vz);
        public bool Repeat => x == x0 && y == y0 && z == z0;
    }
}