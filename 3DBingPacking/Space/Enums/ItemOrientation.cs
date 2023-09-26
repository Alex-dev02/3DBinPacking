using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DBingPacking.Space.NewFolder.Enums
{
    public enum ItemOrientation
    {
        NULL,
        ONE, // l w h
        TWO, // l h w
        THREE, // w l h
        FOUR, // w h l
        FIVE, // h l w
        SIX, // h w l
    }
}
