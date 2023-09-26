using _3DBingPacking.Space.NewFolder.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DBingPacking.Space
{
    public static class GenerateItemOrientations
    {
        private static readonly Random _random = new();
        private static readonly int _enumCount = Enum.GetNames(typeof(ItemOrientation)).Length;
        public static List<ItemOrientation> GetOrientations(
            int length,
            bool randomGenerated = false,
            ItemOrientation orientationForNonRandomGeneration = ItemOrientation.ONE
        )
        {
            List<ItemOrientation> orientations = new();

            for (int i = 0; i < length; i++)
            {
                if (randomGenerated)
                    orientations.Add((ItemOrientation)_random.Next(1, _enumCount));
                else
                    orientations.Add(orientationForNonRandomGeneration);
            }

            return orientations;
        }
    }
}
