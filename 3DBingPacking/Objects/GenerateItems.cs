using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DBingPacking.Objects
{
    public static class GenerateItems
    {
        private static readonly Random random = new();

        public static List<Item> GetItems(int numberOfItems, int maxLength = 150)
        {
            List<Item> items = new();

            for (int i = 0; i < numberOfItems; i++)
            {
                Item item = new(GetRandomSize3D(maxLength));
                items.Add(item);
            }

            return items;
        }

        private static Size3D GetRandomSize3D(int maxLength)
        {
            int length = random.Next(
                (maxLength + 1 - ((maxLength + 1) * 55) / 100),
                (maxLength + 1)
            );

            int width = random.Next(
                (length - (length * 55) / 100),
                (length)
            );

            int height = random.Next(
                (width - (width * 55) / 100),
                (width)
            );

            return new Size3D(length, width, height);
        }
    }
}
