using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DBingPacking.Space
{
    [Serializable]
    public struct Coordinate3D
    {
        public int X;
        public int Y;
        public int Z;

        public Coordinate3D(int x = 0, int y = 0, int z = 0  )
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
