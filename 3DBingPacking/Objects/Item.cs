using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DBingPacking.Objects
{
    [Serializable]
    public class Item
    {
        public Size3D Size { get; set; }

        public Item(Size3D size)
        {
            Size = size;
        }
    }
}
