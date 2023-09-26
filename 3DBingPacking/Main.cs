
using _3DBingPacking.GeneticAlgorithm;
using _3DBingPacking.Objects;
using _3DBingPacking.Space;
using _3DBingPacking.Space.NewFolder.Enums;
using Newtonsoft.Json;


//string pathToItems = @"C:\Users\ionut\OneDrive\Desktop\3DBinPackingVisualisation\items.json";
//List<Item> items = /*GenerateItems.GetItems(50, 90);*/ JsonConvert.DeserializeObject<List<Item>>(File.ReadAllText(pathToItems));
Size3D binSize = new Size3D(350, 200, 150);

// random search solution
/*items = items.OrderByDescending(i => i.Size.Width * i.Size.Length * i.Size.Height).ToList();
List<int> itemsId = Enumerable.Range(0, 20).ToList();

List<ChromosomeToPackingSolution> solutions = new List<ChromosomeToPackingSolution>();

for (int i = 1; i < 50000; i++)
{
    Chromosome chromosome = new Chromosome(itemsId, GenerateItemOrientations.GetOrientations(20, true));
    ChromosomeToPackingSolution cps = new ChromosomeToPackingSolution(1, chromosome, items, binSize);

    solutions.Add(cps);
}

solutions = solutions.OrderByDescending(x => x.GetScore()).ToList();

Console.WriteLine(solutions[0].GetScore());*/

// IGA solution


var items = GenerateItems.GetItems(30, 90);

/*string jsonItems = JsonConvert.SerializeObject(items);
File.WriteAllText(pathToItems, jsonItems);*/

ImprovedGeneticAlgorithm iga = new ImprovedGeneticAlgorithm(ref items, binSize, 500, 300, 100, 100);


string jsonSolution = PackingSolutionToJson.SerializedSolution(
        iga.GetBestPackingSolution().GetPackingSolution(),
        new PackingSolutionToJson.CartonSizeAndCoord() { Size = binSize, Coord = new Coordinate3D(0, 0, 0) }
   );

File.WriteAllText(@"C:\Users\ionut\OneDrive\Desktop\3DBinPackingVisualisation\PackingSolution.json", jsonSolution);
