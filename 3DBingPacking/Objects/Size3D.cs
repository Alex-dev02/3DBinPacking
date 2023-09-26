using _3DBingPacking.Space.NewFolder.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DBingPacking.Objects
{
    [Serializable]
    public struct Size3D
    {
        public int Length;
        public int Width;
        public int Height;

        public Size3D(int length = 0, int width = 0, int height = 0)
        {
            Length = length;
            Width = width;
            Height = height;
        }

        public void ToOrientation(ItemOrientation orientation)
        {
            if (GetOrientation() != ItemOrientation.ONE)
            {
                this.ToOrientationONE();
                // throw new ArgumentException("Cannot convert from other orientation except ONE");

            }

            if (orientation == ItemOrientation.TWO)
            {
                (Width, Height) = (Height, Width);
            } else if (orientation == ItemOrientation.THREE)
            {
               (Length, Width) = (Width, Length);
            } else if (orientation == ItemOrientation.FOUR)
            {
                (Length, Width) = (Width, Length);
                (Width, Height) = (Height, Width);
            } else if (orientation == ItemOrientation.FIVE)
            {
                (Width, Height) = (Height, Width);
                (Length, Width) = (Width, Length);
            } else if (orientation == ItemOrientation.SIX)
            {
                (Length, Height) = (Height, Length);
            }

        }

        public void ToOrientationONE()
        {
            List<int> metrics = new() { Length, Width, Height };
            metrics.Sort();

            Length = metrics[2];
            Width = metrics[1];
            Height = metrics[0];
        }

        public ItemOrientation GetOrientation()
        {
            if (Length >= Width && Width >= Height)
                return ItemOrientation.ONE;
            else if (Length >= Height && Height >= Width)
                return ItemOrientation.TWO;
            else if (Width >= Length && Length >= Height)
                return ItemOrientation.THREE;
            else if (Width >= Height && Height >= Length)
                return ItemOrientation.FOUR;
            else if (Height >= Length && Length >= Height)
                return ItemOrientation.FIVE;
            return ItemOrientation.SIX;
        }
        
    }
}
