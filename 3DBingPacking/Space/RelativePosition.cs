using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DBingPacking.Space
{
    [Serializable]
    public struct RelativePosition
    {
        public int LeftCartonId { get; set; }
        public int FrontCartonId { get; set; }
        public int BottomCartonId { get; set; }

        public RelativePosition(int leftCartonId = -1, int frontCartonId = -1, int bottomCartonId = -1)
        {
            LeftCartonId = leftCartonId;
            FrontCartonId = frontCartonId;
            BottomCartonId = bottomCartonId;
        }
    }
}
