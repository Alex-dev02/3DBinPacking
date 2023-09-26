using _3DBingPacking.Space;
using _3DBingPacking.Space.NewFolder.Enums;

namespace _3DBingPacking.GeneticAlgorithm
{
    [Serializable]
    public class Chromosome
    {
        public List<int> ItemsIds { get; private set; }
        public List<ItemOrientation> ItemsOrientations { get; private set; }

        public Chromosome(List<int> itemsIds, List<ItemOrientation>? itemsOrientations = null)
        {
            if (itemsOrientations == null)
                itemsOrientations = GenerateItemOrientations.GetOrientations(
                    itemsIds.Count,
                    true
                );

            if ((itemsIds.Count == 0) || itemsIds.Count != itemsOrientations?.Count)
                throw new ArgumentException("Count of cartons must be the same as Count of their orientations and NOT 0");
            ItemsIds = itemsIds;
            ItemsOrientations = itemsOrientations;
        }
        public Chromosome DeepClone()
        {
            return new Chromosome(ItemsIds.ToList(), ItemsOrientations.ToList());
        }
    }
}
