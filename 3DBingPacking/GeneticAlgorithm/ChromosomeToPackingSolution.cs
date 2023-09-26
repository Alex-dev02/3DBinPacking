using _3DBingPacking.Objects;
using _3DBingPacking.Space;
using _3DBingPacking.Space.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DBingPacking.GeneticAlgorithm
{
    [Serializable]
    public class ChromosomeToPackingSolution
    {
        public int Id;
        public List<Item> _items; // all public for debugging
        public Chromosome _chromosome;
        public List<Carton> _solution;
        public List<Carton> _initCartons;
        public List<ReferencePoint> _referencePoints;
        public Size3D _cageBinSize;
        public Size3D _binSize;
        public int _packedCartonsVolume;
        public double F1;
        public double F2;
        public double Score { get; set; }

        public ChromosomeToPackingSolution(int id, Chromosome chromosome, List<Item> items, Size3D binSize)
        {
            Id = id;
            _chromosome = chromosome;
            _binSize = binSize;
            _items = items;
            _solution = new List<Carton>();
            _initCartons = new List<Carton>();
            _cageBinSize = new Size3D(0, 0, 0);
            _referencePoints = new List<ReferencePoint>();
            _packedCartonsVolume = 0;

            InitiateCartons();
            InitiateReferencePointsList();
            CreatePackingSolution();

        }
        public Chromosome GetChromosome()
        {
            return _chromosome;
        }
        public double GetScore()
        {
            return Score;
        }
        public List<Carton> GetPackingSolution()
        {
            return _solution;
        }
        private void CreatePackingSolution()
        {
            AssignFirstCartonAtFirstRefPoint();
            AddRefPointsForLastPlacedCarton();
            _cageBinSize =
                    CageBinSizeAfterPlacingCartonAtRefPoint(new ReferencePoint(), _solution[_solution.Count - 1]);
            _packedCartonsVolume = TotalCartonsVolumeAfterPlacingANewOne(_solution[_solution.Count - 1]);

            int initCartonIndex = 1;

            // !!! placing a carton without specifing the RelativePositions will set them to the default -1, -1, -1
            // which may be confused with the bin id, which is also -1 here !!!
            // loop through every initial carton in order to obtain a package solution _solution
            while (initCartonIndex < _initCartons.Count)
            {

                int numberOfFeasableRefPointsForCurrentCarton = 0;

                // for each initial carton, find all the fesable ref points where it can be assigned
                for (int refPointIndex = 0; refPointIndex < _referencePoints.Count; refPointIndex++)
                {
                    
                    // fesable if it doesn't intersect with the bin and other already packed cartons from _solution
                    if (!CartonAtRefPointIntersectsBin(_referencePoints[refPointIndex].Coordinate, _initCartons[initCartonIndex].OrientationalSize)
                        && !CartonIntersectsWithOtherCartonsAlreadyPacked(_referencePoints[refPointIndex].Coordinate, _initCartons[initCartonIndex]))
                    {
                        // mark ref point as fesable (assigned) and add 1 to the counter
                        numberOfFeasableRefPointsForCurrentCarton++;
                        _referencePoints[refPointIndex].IsAssigned = true;

                        // calculate the index1 and index2 values of the fesable ref point for later use
                        // we need these indexes to choose the best ref point available
                        _referencePoints[refPointIndex].Index1 =
                            CalculateIndex1ForRefPointAndCarton(_referencePoints[refPointIndex], _initCartons[initCartonIndex]);

                        // index2 is not always possible to be calculated (if the current cage bin is not big enough
                        // to also contain the new carton placed at the current ref point)
                        // if so, the index2 will return -1
                        _referencePoints[refPointIndex].Index2 =
                            CalculateIndex2ForRefPointAndCarton(_referencePoints[refPointIndex], _initCartons[initCartonIndex]);
                    }
                }

                // after marking all fesable ref points, choose the best one based on index1 and index2
                // if no fesable ref point, pack the carton at the top of the cage bin
                // preparing the type of parallel moves
                ReferencePoint refPointToPlaceCarton = new ReferencePoint();
                if (numberOfFeasableRefPointsForCurrentCarton == 0)
                {
                    // if intersects with everything then add it later in the box
                    /*if (CartonAtRefPointIntersectsBin(new Coordinate3D(0, 0, _cageBinSize.Height), _initCartons[initCartonIndex].OrientationalSize))
                    {
                        _initCartons.Add(_initCartons[initCartonIndex]);
                        _initCartons.RemoveAt(initCartonIndex);
                        continue;
                    }*/
                    //throw new Exception("hi");
                    PlaceCartonAtCoordinate(_initCartons[initCartonIndex], new Coordinate3D(0, 0, _cageBinSize.Height));
                    refPointToPlaceCarton.Type = ReferencePointType.Z_ONLY;
                    refPointToPlaceCarton.Coordinate = new Coordinate3D(0, 0, _cageBinSize.Height); // ????
                }
                else
                {
                    refPointToPlaceCarton =
                        GetBestRefPointToPlaceCarton();

                    PlaceCartonAtCoordinate(_initCartons[initCartonIndex], refPointToPlaceCarton.Coordinate);
                    RestoreRefPointListAfterPlacingCarton(refPointToPlaceCarton);
                }

                // parallel moves // COMPLETE IT
                ParallelMoveLastAddedCarton(refPointToPlaceCarton.Type);

                // add three more ref points coresponding to the new packed carton
                AddRefPointsForLastPlacedCarton();

                // read the diagram to make sure you have completed all the steps 


                // also update the cage bin I guess?
                _cageBinSize =
                    CageBinSizeAfterPlacingCartonAtRefPoint(refPointToPlaceCarton, _solution[_solution.Count - 1]);

                // also update the volume of the cartons packed so far?
                _packedCartonsVolume = TotalCartonsVolumeAfterPlacingANewOne(_solution[_solution.Count - 1]);

                initCartonIndex++;
            }

            F1 = CalculateF1();
            F2 = CalculateF2();
            Score = F1 + F2;

        }
        private double CalculateF1()
        {
            return (double)((double)_packedCartonsVolume / (double)(_binSize.Length * _binSize.Width * _binSize.Height)) * 10000;
        }
        private double CalculateF2()
        {
            return (double)((double)_packedCartonsVolume / (double)(_cageBinSize.Length * _cageBinSize.Width * _cageBinSize.Height)) * 10000; 
        }
        private void AddRefPointsForLastPlacedCarton()
        {
            Carton lastAddedCartonToSolution = _solution[_solution.Count - 1];

            Coordinate3D coordForType1RefPoint = lastAddedCartonToSolution.Coordinate;
            coordForType1RefPoint.X += lastAddedCartonToSolution.OrientationalSize.Length;

            _referencePoints.Add(
                new ReferencePoint(coordForType1RefPoint, lastAddedCartonToSolution.ItemId, ReferencePointType.ONE)
                );

            Coordinate3D coordForType2RefPoint = lastAddedCartonToSolution.Coordinate;
            coordForType2RefPoint.Y += lastAddedCartonToSolution.OrientationalSize.Width;

            _referencePoints.Add(
                new ReferencePoint(coordForType2RefPoint, lastAddedCartonToSolution.ItemId, ReferencePointType.TWO)
                );

            Coordinate3D coordForType3RefPoint = lastAddedCartonToSolution.Coordinate;
            coordForType3RefPoint.Z += lastAddedCartonToSolution.OrientationalSize.Height;

            _referencePoints.Add(
                new ReferencePoint(coordForType3RefPoint, lastAddedCartonToSolution.ItemId, ReferencePointType.THREE)
                );
        } 
        private void AssignFirstCartonAtFirstRefPoint()
        {
            PlaceCartonAtCoordinate(_initCartons[0], _referencePoints[0].Coordinate);
            RestoreRefPointListAfterPlacingCarton(_referencePoints[0]);
        }
        private void ParallelMoveLastAddedCarton(ReferencePointType type)
        {

        }
        private void RestoreRefPointListAfterPlacingCarton(ReferencePoint refPointNewlyOccupied)
        {
            DeleteRefPointFromList(refPointNewlyOccupied);
            UnassignRefPointsFromList();
        }
        public void UnassignRefPointsFromList()
        {
            for (int i = 0; i < _referencePoints.Count; i++)
                _referencePoints[i].IsAssigned = false;
        }
        private void DeleteRefPointFromList(ReferencePoint refPointToDelete)
        {
            _referencePoints.Remove(refPointToDelete);
        }
        private ReferencePoint GetBestRefPointToPlaceCarton()
        {
            ReferencePoint? refPointWithLowestIndex2 = null;
            // _referencePoints.Where(r => r.Index2 != -1 && r.IsAssigned).MinBy(r => r.Index2);
            ReferencePoint? refPointWithLowestIndex1 = null;
            //_referencePoints.Where(r => r.IsAssigned).MinBy(r => r.Index1);


            for (int i = 0; i < _referencePoints.Count; i++)
            {
                if (!_referencePoints[i].IsAssigned)
                    continue;

                if (_referencePoints[i].Index2 != -1  && (refPointWithLowestIndex2 == null || refPointWithLowestIndex2.Index2 < _referencePoints[i].Index2))
                    refPointWithLowestIndex2 = _referencePoints[i];

                if (refPointWithLowestIndex1 == null || refPointWithLowestIndex1.Index1 < _referencePoints[i].Index1)
                    refPointWithLowestIndex1 = _referencePoints[i];
            }

            if (refPointWithLowestIndex2 != null)
                return refPointWithLowestIndex2;

            if (refPointWithLowestIndex1 != null)
                return refPointWithLowestIndex1;

            throw new Exception("Could not find an appropiate ref point!");
        }
        private void PlaceCartonAtCoordinate(Carton carton, Coordinate3D placePoint)
        {
            carton.Coordinate = placePoint;
            _solution.Add(carton);
        }
        private double CalculateIndex1ForRefPointAndCarton(ReferencePoint refPoint, Carton carton)
        {
            Size3D cageBinSizeAfterPlacingCartonAtRefPoint = CageBinSizeAfterPlacingCartonAtRefPoint(refPoint, carton);
            int newVolumeOfCartonsPacked = TotalCartonsVolumeAfterPlacingANewOne(carton);

            // (length * width * height^2) / volume

            return (cageBinSizeAfterPlacingCartonAtRefPoint.Length * cageBinSizeAfterPlacingCartonAtRefPoint.Width
                    * (cageBinSizeAfterPlacingCartonAtRefPoint.Height * cageBinSizeAfterPlacingCartonAtRefPoint.Height))
                    / newVolumeOfCartonsPacked;  
        }
        private double CalculateIndex2ForRefPointAndCarton(ReferencePoint refPoint, Carton carton)
        {
            if (CartonPlacedAtRefPointIntersectsWithCurrentCageBin(refPoint, carton))
                return -1;

            int residualLength = _cageBinSize.Length - refPoint.Coordinate.X;
            int residualWidth = _cageBinSize.Width - refPoint.Coordinate.Y;
            int residualHeight = _cageBinSize.Height - refPoint.Coordinate.Z;

            // index2 = (ls * ws * (hs^2)) / (ln * wn * hn);
            return (residualLength * residualWidth * (residualHeight * residualHeight))
                    / (carton.OrientationalSize.Length * carton.OrientationalSize.Width * carton.OrientationalSize.Height);
        }
        private bool CartonPlacedAtRefPointIntersectsWithCurrentCageBin(ReferencePoint refPoint, Carton carton)
        {
            return (refPoint.Coordinate.X + carton.OrientationalSize.Length > _cageBinSize.Length)
                    ||
                   (refPoint.Coordinate.Y + carton.OrientationalSize.Width > _cageBinSize.Width)
                    ||
                   (refPoint.Coordinate.Z + carton.OrientationalSize.Height > _cageBinSize.Height);
        }
        private int TotalCartonsVolumeAfterPlacingANewOne(Carton newCarton)
        {
            return _packedCartonsVolume + (newCarton.OriginalSize.Length * newCarton.OriginalSize.Width * newCarton.OriginalSize.Height)/100;
        }
        private Size3D CageBinSizeAfterPlacingCartonAtRefPoint(ReferencePoint refPoint, Carton carton)
        {
            int newCageBinLength = 0;
            int newCageBinWidth =  0;
            int newCageBinHeight = 0;

            if (refPoint.Coordinate.X + carton.OrientationalSize.Length > _cageBinSize.Length)
                newCageBinLength = refPoint.Coordinate.X + carton.OrientationalSize.Length;

            if (refPoint.Coordinate.Y + carton.OrientationalSize.Width > _cageBinSize.Width)
                newCageBinWidth = refPoint.Coordinate.Y + carton.OrientationalSize.Width;

            if (refPoint.Coordinate.Z + carton.OrientationalSize.Height > _cageBinSize.Height)
                newCageBinHeight = refPoint.Coordinate.Z + carton.OrientationalSize.Height;

            return new Size3D(
                newCageBinLength != 0 ? newCageBinLength : _cageBinSize.Length,
                newCageBinWidth != 0 ? newCageBinWidth : _cageBinSize.Width,
                newCageBinHeight != 0 ? newCageBinHeight : _cageBinSize.Height
                );
        }
        private bool CartonIntersectsWithOtherCartonsAlreadyPacked(Coordinate3D refPointCoord, Carton carton)
        {
            carton.Coordinate = refPointCoord;
            int solCount = _solution.Count;

            for (int i = 0; i < solCount; i++)
            {
                if (CartonsIntersect(_solution[i], carton))
                    return true;
            }

            return false;
        }
        public bool CartonsIntersect(Carton c1, Carton c2)
        {
            if (
                (c1.Coordinate.X + c1.OrientationalSize.Length > c2.Coordinate.X && c1.Coordinate.X < c2.Coordinate.X + c2.OrientationalSize.Length) &&
                (c1.Coordinate.Y + c1.OrientationalSize.Width > c2.Coordinate.Y && c1.Coordinate.Y < c2.Coordinate.Y + c2.OrientationalSize.Width) &&
                (c1.Coordinate.Z + c1.OrientationalSize.Height > c2.Coordinate.Z && c1.Coordinate.Z < c2.Coordinate.Z + c2.OrientationalSize.Height)
            )
            {
                return true;
            }

            return false;
        }
        private bool CartonAtRefPointIntersectsBin(Coordinate3D refPointKCoords, Size3D cartonOrientationalSize)
        {
            if (
                refPointKCoords.X + cartonOrientationalSize.Length <= _binSize.Length
                &&
                refPointKCoords.Y + cartonOrientationalSize.Width <= _binSize.Width
                &&
                refPointKCoords.Z + cartonOrientationalSize.Height <= _binSize.Height
                )
            {
                return false;
            }

            return true;
        }
        private void InitiateCartons()
        {
            for (int i = 0; i < _chromosome?.ItemsIds.Count; i++)
            {
                Size3D orientationalSize = _items[_chromosome.ItemsIds[i]].Size;
                orientationalSize.ToOrientation(_chromosome.ItemsOrientations[i]);
                _initCartons.Add(
                    new Carton(
                        _chromosome.ItemsIds[i],
                        _chromosome.ItemsOrientations[i],
                        _items[_chromosome.ItemsIds[i]].Size,
                        orientationalSize
                    )
                );
            }
        }
        private void InitiateReferencePointsList()
        {
            _referencePoints.Add(new ReferencePoint(
                new Coordinate3D(0, 0, 0),
                -1,
                Space.Enums.ReferencePointType.NULL
            ));
        }
    }
}
