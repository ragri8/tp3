using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Map {
    public class GameGrid {
        private List<Gridbox> gridboxes;
        private int sizeX;
        private int sizeZ;
        public static int gridBoxSize = 10;

        public GameGrid(int sizeX = 30, int sizeZ = 30) {
            this.sizeX = sizeX;
            this.sizeZ = sizeZ;
            gridboxes = new List<Gridbox>();
            for (var z = 0; z < sizeZ; z++) {
                for (var x = 0; x < sizeX; x++) {
                    var gridBox = new Gridbox(coordToIndex(x, z), x, z);
                    gridboxes.Add(gridBox);
                }
            }
        }

        public int getSize() {
            return sizeX * sizeZ;
        }

        public void linkBoxes(int first, int second) {
            gridboxes[first].addConnection(second);
            gridboxes[second].addConnection(first);
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

        public int realWorldCoordToIndex(float realPosX, float realPosZ, int zeroPosX, int zeroPosZ) {
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
                south = gridboxes[index].isConnected(coordToIndex(posX, posZ - 1));
            }
            var north = false;
            if (boxExist(posX, posZ + 1)) {
                north = gridboxes[index].isConnected(coordToIndex(posX, posZ + 1));
            }
            var east = false;
            if (boxExist(posX + 1, posZ)) {
                east = gridboxes[index].isConnected(coordToIndex(posX + 1, posZ));
            }
            var west = false;
            if (boxExist(posX - 1, posZ)) {
                west = gridboxes[index].isConnected(coordToIndex(posX - 1, posZ));
            }
            return new Tuple<bool, bool, bool, bool>(south, north, east, west);
        }

        public Vector3 randomPosition(float height) {
            var index = Random.Range(0, sizeX * sizeZ - 1);
            return indexToRealWorldCoord(index, height);
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
    }
}
