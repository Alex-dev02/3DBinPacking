using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DBingPacking.Objects
{
    public static class LoadItemsJson
    {
        private static string _path = @"C:\Users\ionut\OneDrive\Desktop\3DBinPackingVisualisation\PackingSolution.json";

        public static List<Item> LoadJson()
        {
            List<PackingSolutionToJson.CartonSizeAndCoord> solution =
                JsonConvert.DeserializeObject<List<PackingSolutionToJson.CartonSizeAndCoord>>(File.ReadAllText(_path));

            List<Item> res = new List<Item>();

            for (int i = 0; i < solution.Count; i++)
            {
                res.Add(new Item(solution[i].Size));
            }

            res.RemoveAt(0);

            return res;
        }
    }
}
