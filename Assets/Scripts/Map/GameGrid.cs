using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Map {
    public class GameGrid {
        private List<Gridbox> gridSquares;
        private int sizeX;
        private int sizeZ;
        public static int gridBoxSize = 10;
        private int[,] gridSquareDistances;
        private int[,] nextToIndexLists;

        public GameGrid(int sizeX = 30, int sizeZ = 30) {
            this.sizeX = sizeX;
            this.sizeZ = sizeZ;
            gridSquares = new List<Gridbox>();
            for (var z = 0; z < sizeZ; z++) {
                for (var x = 0; x < sizeX; x++) {
                    var gridBox = new Gridbox(coordToIndex(x, z), x, z);
                    gridSquares.Add(gridBox);
                }
            }
        }

        /*
         * Create a full matrix of distance between each gridSquares
         */
        public void initializeDistances() {
            // adjacent lists for each index
            nextToIndexLists = new int[getSize(), getSize()];
            // give default max value
            gridSquareDistances = new int[getSize(), getSize()];
            for (var i = 0; i < getSize(); i++) {
                for (var j = 0; j < getSize(); j++) {
                    nextToIndexLists[i, j] = getSize();
                    gridSquareDistances[i, j] = getSize();
                }
            }
            // set distance between adjacents gridSquares
            var adjacentIndexLists = new List<List<int>>();
            for (var index = 0; index < getSize(); index++) {
                // set to zero distance with self
                gridSquareDistances[index, index] = 0;
                adjacentIndexLists.Add(gridSquares[index].getAdjacents());
                foreach (var adjacentIndex in gridSquares[index].getAdjacents()) {
                    gridSquareDistances[index, adjacentIndex] = 1;
                    nextToIndexLists[index, adjacentIndex] = adjacentIndex;
                }
            }
            // set list of index having an exact distance from another one
            // the list is modified on each incrementation based on distance variable
            var fixedDistanceAdjacentIndexLists = new List<List<int>>(adjacentIndexLists);
            // set final distance with incremental approach, greedy algorithm (m^2?)
            for (var distance = 1; distance < getSize(); distance++) {
                var tempFixedDistanceAdjacentIndexLists = new List<List<int>>();
                for (var firstIndex = 0; firstIndex < getSize(); firstIndex++) {
                    var tempFixedDistanceAdjacentIndexes = new List<int>();
                    foreach (var secondIndex in adjacentIndexLists[firstIndex]) {
                        foreach (var thirdIndex in fixedDistanceAdjacentIndexLists[secondIndex]) {
                            if (firstIndex == thirdIndex) {
                                continue;
                            }
                            if (distance + 1 < gridSquareDistances[firstIndex, thirdIndex]) {
                                gridSquareDistances[firstIndex, thirdIndex] = distance + 1;
                                tempFixedDistanceAdjacentIndexes.Add(thirdIndex);
                                nextToIndexLists[firstIndex, thirdIndex] = secondIndex;
                            }
                        }
                    }
                    tempFixedDistanceAdjacentIndexLists.Add(tempFixedDistanceAdjacentIndexes);
                }
                fixedDistanceAdjacentIndexLists = tempFixedDistanceAdjacentIndexLists;
            }
        }

        public int getIndexDistance(int first, int second) {
            return gridSquareDistances[first, second];
        }

        public int getNextToIndex(int first, int second) {
            return nextToIndexLists[first, second];
        }

        public int getSize() {
            return sizeX * sizeZ;
        }

        public void linkBoxes(int first, int second) {
            gridSquares[first].addConnection(second);
            gridSquares[second].addConnection(first);
        }

        private bool boxExist(int posX, int posZ) {
            return 0 <= posX && posX < sizeX && 0 <= posZ && posZ < sizeZ;
        }
        private bool boxExist(int index) {
            return 0 <= index && index < sizeX * sizeZ;
        }

        public Tuple<int, int> indexToCoord(int index) {
            return new Tuple<int, int>(index % sizeX, index / sizeX);
        }
        
        public int coordToIndex(int posX, int posZ) {
            return posX + posZ * sizeX;
        }

        public int realWorldCoordToIndex(float realPosX, float realPosZ, int zeroPosX=0, int zeroPosZ=0) {
            var posX = (int) (realPosX - zeroPosX) / gridBoxSize;
            var posZ = (int) (realPosZ - zeroPosZ) / gridBoxSize;
            return coordToIndex(posX, posZ);
        }
        public Vector3 indexToRealWorldCoord(int index, float height) {
            var pos2D = indexToCoord(index);
            var posX = pos2D.Item1 * gridBoxSize + gridBoxSize / 2.0f;
            var posY = height;
            var posZ = pos2D.Item2 * gridBoxSize + gridBoxSize / 2.0f;
            return new Vector3(posX, posY, posZ);
        }

        /**
         * tuple of connexions between a gridBox and its surrounding
         * each bool represent a direction and specify if it is connected to another box
         * south, north, east, west
         */
        public Tuple<bool, bool, bool, bool> connectedInDirections(int index) {
            var coord = indexToCoord(index);
            var posX = coord.Item1;
            var posZ = coord.Item2;
            var south = false;
            if (boxExist(posX, posZ - 1)) {
                south = gridSquares[index].isConnected(coordToIndex(posX, posZ - 1));
            }
            var north = false;
            if (boxExist(posX, posZ + 1)) {
                north = gridSquares[index].isConnected(coordToIndex(posX, posZ + 1));
            }
            var east = false;
            if (boxExist(posX + 1, posZ)) {
                east = gridSquares[index].isConnected(coordToIndex(posX + 1, posZ));
            }
            var west = false;
            if (boxExist(posX - 1, posZ)) {
                west = gridSquares[index].isConnected(coordToIndex(posX - 1, posZ));
            }
            return new Tuple<bool, bool, bool, bool>(south, north, east, west);
        }

        public Vector3 randomPosition(float height) {
            var index = Random.Range(0, sizeX * sizeZ - 1);
            return indexToRealWorldCoord(index, height);
        }

        public Vector3 getRandomAdjacent(float posX, float posY, float posZ) {
            var currentIndex = realWorldCoordToIndex(posX, posZ);
            var randomAdjacentIndex = gridSquares[currentIndex].getAdjacents()[
                Random.Range(0, gridSquares[currentIndex].getAdjacentCount() - 1)];
            return indexToRealWorldCoord(randomAdjacentIndex, posY);
        }

        public Vector3 randomPositionInRange(int refIndex, int minRange, int maxRange, float height) {
            var randomDistance = Random.Range(minRange, maxRange);
            var refCoord = indexToCoord(refIndex);
            var posX = Random.Range(
                Math.Max(refCoord.Item1 - randomDistance, 0),
                Math.Min(refCoord.Item1 + randomDistance, sizeX - 1));
            var remaining = randomDistance - Math.Abs(refCoord.Item1 - posX);
            var posZPossibilities = new List<int>();
            if (refCoord.Item2 - remaining >= 0) {
                posZPossibilities.Add(refCoord.Item2 - remaining);
            }
            if (refCoord.Item2 + remaining < sizeZ) {
                posZPossibilities.Add(refCoord.Item2 + remaining);
            }
            var posZ = posZPossibilities[Random.Range(0, posZPossibilities.Count - 1)];
            var index = coordToIndex(posX, posZ);
            return indexToRealWorldCoord(index, height);
        }
        
        public List<List<int>> findIndexesWithinDistance(int startIndex, int previousIndex, int range) {
            var paths = new List<List<int>>();

            foreach (var nextIndex in gridSquares[startIndex].getAdjacents()) {
                if (nextIndex == previousIndex) {
                    continue;
                }

                if (range == 1) {
                    paths.Add(new List<int>(nextIndex));
                } else {
                    var tempPathLists = findIndexesWithinDistance(
                        nextIndex,
                        startIndex,
                        range - 1);
                    if (tempPathLists.Count == 0) {
                        paths.Add(new List<int>(nextIndex));
                        continue;
                    }
                    
                    foreach (var tempPath in tempPathLists) {
                        tempPath.Insert(0, nextIndex);
                        paths.Add(tempPath);
                    }
                }
            }

            if (paths.Count == 0) {
                paths.Add(new List<int>());
            }
            
            return paths;
        }
    }
}
