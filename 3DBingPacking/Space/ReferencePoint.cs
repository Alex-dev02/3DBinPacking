using _3DBingPacking.Space.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DBingPacking.Space
{
    [Serializable]
    public class ReferencePoint
    {
        public Coordinate3D Coordinate { get; set; }
        public int AdjacentCartonId { get; set; }
        public ReferencePointType Type { get; set; }
        public double Index1 { get; set; }
        public double Index2 { get; set; }
        public bool IsAssigned { get; set; }

        public ReferencePoint(
            Coordinate3D coordinate = default,
            int adjacentCartonId = 0, // -1 reserved for the bin
            ReferencePointType type = ReferencePointType.NULL,
            float index1 = 0,
            float index2 = -1,
            bool isAssigned = false
        )
        {
            Coordinate = coordinate;
            AdjacentCartonId = adjacentCartonId;
            Type = type;
            Index1 = index1;
            Index2 = index2;
            IsAssigned = isAssigned;
        }
    }
}
