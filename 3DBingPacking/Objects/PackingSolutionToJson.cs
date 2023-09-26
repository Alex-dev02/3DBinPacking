using _3DBingPacking.Space;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DBingPacking.Objects
{
    public static class PackingSolutionToJson
    {
        public class CartonSizeAndCoord
        {
            public Size3D Size { get; set; }
            public Coordinate3D Coord { get; set; }
        }
        public static string SerializedSolution(List<Carton> solution, CartonSizeAndCoord bin)
        {
            var s = SimplifySolution(ref solution);
            s.Insert(0, bin);
            return JsonConvert.SerializeObject(s, Formatting.Indented);
        }
        private static List<CartonSizeAndCoord> SimplifySolution(ref List<Carton> solution)
        {
            List<CartonSizeAndCoord> simplifiedSolution = new List<CartonSizeAndCoord>();

            for (int i = 0; i < solution.Count; i++)
                simplifiedSolution.Add(
                    new CartonSizeAndCoord() { Size = solution[i].OrientationalSize, Coord = solution[i].Coordinate }
                );

            return simplifiedSolution;
        }
    }
}
