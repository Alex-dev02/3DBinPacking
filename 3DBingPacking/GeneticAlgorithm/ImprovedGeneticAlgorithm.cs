using _3DBingPacking.Objects;
using _3DBingPacking.Space;
using _3DBingPacking.Space.NewFolder.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DBingPacking.GeneticAlgorithm
{
    public class ImprovedGeneticAlgorithm
    {
        private class CrossoverItem
        {
            public int ItemId { get; set; }
            public ItemOrientation Orientation { get; set; }
            
/*            public static bool operator==(CrossoverItem item1, CrossoverItem item2)
            {
                return item1.ItemId == item2.ItemId;
            }
            public static bool operator!=(CrossoverItem item1, CrossoverItem item2)
            {
                return !(item1 == item2);
            }*/
        }
        const int _orientationsToMutate = 3;
        private List<Chromosome> _chromosomes;
        private List<ChromosomeToPackingSolution> _solutions;
        private ChromosomeToPackingSolution _best;
        private List<Item> _itemsToPack;
        private Size3D _binSize;
        private int _populationSize;
        private int _crossoverSize;
        private int _mutationSize;
        private int _tournamentSize;
        private Random _random;

        public ImprovedGeneticAlgorithm(ref List<Item> itemsToPack, Size3D binSize, int populationSize = 10, int crossoverSize = 10, int mutationSize = 1, int tournamentSize = 5)
        {
            _chromosomes = new List<Chromosome>();
            _solutions = new List<ChromosomeToPackingSolution>();
            _itemsToPack = itemsToPack;
            _binSize = binSize;
            _populationSize = populationSize;
            _crossoverSize = crossoverSize;
            _mutationSize = mutationSize;
            _tournamentSize = tournamentSize;
            _random = new Random();

            // Creating the initial population
            AddFirst6ChromosomesToPopulation();
            AddRemainderChromosomes();
            EvaluatePopulation();
            EvolvePopulation();

        }
        public ChromosomeToPackingSolution GetBestPackingSolution()
        {
            /*_solutions.Sort((ChromosomeToPackingSolution c1, ChromosomeToPackingSolution c2) =>
            {
                return c2.GetScore().CompareTo(c1.GetScore());
            });

            return _solutions[0];*/

            return _best;
        }
        private void EvolvePopulation()
        {
            List<Chromosome> selectedChromosomesForCrossover = new List<Chromosome>();
            List<Chromosome> selectedChromosomesForMutation = new List<Chromosome>();

            double best;
            double worst;
            int i = 0;
            do
            {
                selectedChromosomesForCrossover = SelectChromosomesForCrossOver();
                selectedChromosomesForMutation = SelectChromosomesForMutation();
                CrossoverChromosomes(ref selectedChromosomesForCrossover);
                MutateChromosomes(ref selectedChromosomesForMutation);

                Tuple<double, double> bestAndWorst = EvaluatePopulation();
                best = bestAndWorst.Item1;
                worst = bestAndWorst.Item2;
                Console.WriteLine(best);
                Console.WriteLine(worst);
                i++;
            } while (i < 200);

            Console.WriteLine("The best score: " + _best.GetScore().ToString());
        }
        private Chromosome MutateChromosome(Chromosome c)
        {
            int indexItem1 = _random.Next(0, _itemsToPack.Count);
            int indexItem2 = _random.Next(0, _itemsToPack.Count);

            // if indexes are the same
            if (indexItem1 == indexItem2)
            {
                if (indexItem2 == _itemsToPack.Count - 1)
                    indexItem2--;
                else
                    indexItem2++;
            }

            (c.ItemsIds[indexItem1], c.ItemsIds[indexItem2]) = (c.ItemsIds[indexItem2], c.ItemsIds[indexItem1]);
            (c.ItemsOrientations[indexItem1], c.ItemsOrientations[indexItem2]) = (c.ItemsOrientations[indexItem2], c.ItemsOrientations[indexItem1]);

            for (int i = 0; i < _orientationsToMutate; i++)
            {
                c.ItemsOrientations[_random.Next(0, c.ItemsOrientations.Count)] = (ItemOrientation)_random.Next(1, 7);
            }

            return c;
        }
        private void MutateChromosomes(ref List<Chromosome> selectedChromosomesForMutation)
        {
            for (int i = 0; i < selectedChromosomesForMutation.Count; i++)
            {
                _chromosomes.Add(MutateChromosome(selectedChromosomesForMutation[i]));
            }
        }
        private List<Chromosome> SelectChromosomesForMutation()
        {
            List<Chromosome> selectedChromosomesForMutation = new List<Chromosome>();

            for (int i = 0; i < _mutationSize; i++)
            {
                selectedChromosomesForMutation.Add(TournamentSelection(false));
            }

            return selectedChromosomesForMutation;
        }
        private void CrossoverChromosomes(ref List<Chromosome> selectedChromosomesForCrossover)
        {

            for (int i = 0; i < _crossoverSize; i += 2)
            {
                var offsprings = Crossover(selectedChromosomesForCrossover[i], selectedChromosomesForCrossover[i + 1]);

                // uncomment for elitism
                var f1 = new ChromosomeToPackingSolution(i, selectedChromosomesForCrossover[i], _itemsToPack, _binSize);
                var f2 = new ChromosomeToPackingSolution(i, selectedChromosomesForCrossover[i + 1], _itemsToPack, _binSize);

                var s1 = new ChromosomeToPackingSolution(i, offsprings.Item1, _itemsToPack, _binSize);
                var s2 = new ChromosomeToPackingSolution(i, offsprings.Item2, _itemsToPack, _binSize);

                if (f1.GetScore() >= s1.GetScore())
                    _chromosomes.Add(selectedChromosomesForCrossover[i]);
                else if (f1.GetScore() < s1.GetScore())
                    _chromosomes.Add(offsprings.Item1);

                if (f2.GetScore() >= s2.GetScore())
                    _chromosomes.Add(selectedChromosomesForCrossover[i + 1]);
                else if (f2.GetScore() < s2.GetScore())
                    _chromosomes.Add(offsprings.Item2);

                // uncomment for difersity
                /*_chromosomes.Add(offsprings.Item1);
                _chromosomes.Add(offsprings.Item2);*/
            }
        }
        public Tuple<Chromosome, Chromosome> Crossover(Chromosome parent1, Chromosome parent2)
        {
            double divider = (double)(1f / 3f);
            int geneMutationSize = (int)(_itemsToPack.Count * divider);
            int crossoverStartIndex = _random.Next(0, _itemsToPack.Count - geneMutationSize);
            int crossoverStopIndex = crossoverStartIndex + geneMutationSize;

            List<CrossoverItem> itemsChromosome1 = new List<CrossoverItem>();
            List<CrossoverItem> itemsChromosome2 = new List<CrossoverItem>();

            Chromosome offspring1 = parent1.DeepClone();
            Chromosome offspring2 = parent2.DeepClone();

            for (int i = crossoverStartIndex; i < crossoverStopIndex; i++)
            {
                offspring1.ItemsIds[i] = parent2.ItemsIds[i];
                offspring2.ItemsIds[i] = parent1.ItemsIds[i];

                offspring1.ItemsOrientations[i] = parent2.ItemsOrientations[i];
                offspring2.ItemsOrientations[i] = parent1.ItemsOrientations[i];

                itemsChromosome1.Add(
                    new CrossoverItem() { ItemId = parent1.ItemsIds[i], Orientation = parent1.ItemsOrientations[i] }
                    );
                itemsChromosome2.Add(
                    new CrossoverItem() { ItemId = parent2.ItemsIds[i], Orientation = parent2.ItemsOrientations[i] }
                    );
            }

            List<CrossoverItem> specificParent1Items = new List<CrossoverItem>();//itemsChromosome1.Except(itemsChromosome2, (x, y) => x == y).OrderBy(x => x.ItemId).ToList();
            List<CrossoverItem> specificParent2Items = new List<CrossoverItem>();//itemsChromosome2.Except(itemsChromosome1).OrderBy(x => x.ItemId).ToList();

            for (int i = 0; i < itemsChromosome1.Count; i++)
                if (itemsChromosome2.FirstOrDefault(x => x.ItemId == itemsChromosome1[i].ItemId) == null)
                    specificParent1Items.Add(itemsChromosome1[i]);

            for (int i = 0; i < itemsChromosome2.Count; i++)
                if (itemsChromosome1.FirstOrDefault(x => x.ItemId == itemsChromosome2[i].ItemId) == null)
                    specificParent2Items.Add(itemsChromosome2[i]);

            specificParent1Items.Sort((x, y) =>
            {
                return x.ItemId.CompareTo(y.ItemId);
            });

            specificParent2Items.Sort((x, y) =>
            {
                return x.ItemId.CompareTo(y.ItemId);
            });

            for (int i = 0; i < _itemsToPack.Count; i++)
            {
                for (int j = 0; j < specificParent1Items.Count; j++)
                {
                    if (i >= crossoverStartIndex && i < crossoverStopIndex)
                        break;

                    if (offspring1.ItemsIds[i] == specificParent2Items[j].ItemId)
                    {
                        offspring1.ItemsIds[i] = specificParent1Items[j].ItemId;
                        offspring1.ItemsOrientations[i] = specificParent1Items[j].Orientation;
                    }
                    if (offspring2.ItemsIds[i] == specificParent1Items[j].ItemId)
                    {
                        offspring2.ItemsIds[i] = specificParent2Items[j].ItemId;
                        offspring2.ItemsOrientations[i] = specificParent2Items[j].Orientation;
                    }
                }
            }

            return new Tuple<Chromosome, Chromosome>(offspring1, offspring2);
        }
        private List<Chromosome> SelectChromosomesForCrossOver()
        {
            List<Chromosome> selectedChromosomesForCrossover = new List<Chromosome>();

            for (int i = 0; i < _crossoverSize; i++)
            {
                selectedChromosomesForCrossover.Add(TournamentSelection(true));
            }

            return selectedChromosomesForCrossover;
        }
        private Chromosome TournamentSelection(bool selectBest)
        {
            ChromosomeToPackingSolution? chromosomePackingSolution = null;
            int indexOfSelectedPackingSolution = 0;

            for (int i = 0; i < _tournamentSize; i++)
            {
                int packingSolutionIndex = _random.Next(0, _solutions.Count);

                if (chromosomePackingSolution == null || (selectBest && _solutions[packingSolutionIndex].GetScore() > chromosomePackingSolution.GetScore()))
                {
                    chromosomePackingSolution = _solutions[packingSolutionIndex];
                    indexOfSelectedPackingSolution = packingSolutionIndex;
                }
                else if (!selectBest && _solutions[packingSolutionIndex].GetScore() < chromosomePackingSolution.GetScore())
                {
                    chromosomePackingSolution = _solutions[packingSolutionIndex];
                    indexOfSelectedPackingSolution = packingSolutionIndex;
                }
            }
            
            _solutions.RemoveAt(indexOfSelectedPackingSolution);
            _chromosomes.RemoveAt(indexOfSelectedPackingSolution);

            
            return chromosomePackingSolution.GetChromosome();
        }
        private Tuple<double, double> EvaluatePopulation()
        {
            double best = 0;
            double worst = 200;
            int indexOfBest = 0;
            _solutions.Clear();

            for (int i = 0; i < _chromosomes.Count; i++)
            {
                _solutions.Add(new ChromosomeToPackingSolution(
                    i, _chromosomes[i], _itemsToPack, _binSize
                    ));

                if (_solutions[i].GetScore() > best)
                {
                    best = _solutions[i].GetScore();
                    indexOfBest = i;
                }
                if (_solutions[i].GetScore() < worst)
                    worst = _solutions[i].GetScore();
            }

            if (_best == null || _solutions[indexOfBest].GetScore() > _best.GetScore())
                _best = _solutions[indexOfBest].Deep();

            return new Tuple<double, double>(best, worst);
        }
        // Creating the initial population
        private void AddFirst6ChromosomesToPopulation()
        {
            List<int> itemIds = Enumerable.Range(0, _itemsToPack.Count).ToList();
            itemIds = itemIds.OrderByDescending(
                x =>
                    _itemsToPack[x].Size.Length * _itemsToPack[x].Size.Width * _itemsToPack[x].Size.Height
                ).ToList();

            for (int i = 0; i < 6; i++)
            {
                _chromosomes.Add(
                    new Chromosome(itemIds.ToList(), GenerateItemOrientations.GetOrientations(_itemsToPack.Count, false, (ItemOrientation)(i + 1)))
                    );
            }
        }
        private void AddRemainderChromosomes()
        {
            int noOfChomosomesLeftToAdd = _populationSize - 6;
            List<int> itemIds = Enumerable.Range(0, _itemsToPack.Count).ToList();
            itemIds = itemIds.OrderByDescending(
                x =>
                    _itemsToPack[x].Size.Length * _itemsToPack[x].Size.Width * _itemsToPack[x].Size.Height
                ).ToList();
            for (int i = 0; i < noOfChomosomesLeftToAdd; i++)
            {
                //itemIds.Shuffle();
                _chromosomes.Add(
                    new Chromosome(itemIds.ToList(), GenerateItemOrientations.GetOrientations(_itemsToPack.Count, true))
                    );
            }

        }


    }
}
