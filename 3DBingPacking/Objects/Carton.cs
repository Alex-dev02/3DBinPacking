using _3DBingPacking.Space;
using _3DBingPacking.Space.NewFolder.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DBingPacking.Objects
{
    [Serializable]
    public class Carton
    {
        public int ItemId { get; set; }
        public ItemOrientation Orientation { get; set; }
        public Size3D OriginalSize { get; set; }
        public Size3D OrientationalSize;
        public Coordinate3D Coordinate;
        public RelativePosition RelativePosition { get; set; }

        public Carton(
            int itemId = 0,
            ItemOrientation orientation = ItemOrientation.NULL,
            Size3D originalSize = default,
            Size3D orientationalSize = default,
            Coordinate3D coordinate = default,
            RelativePosition relativePosition = default
        )
        {
            ItemId = itemId;
            Orientation = orientation;
            OriginalSize = originalSize;
            OrientationalSize = orientationalSize;
            Coordinate = coordinate;
            RelativePosition = relativePosition;
        }
    }
}
