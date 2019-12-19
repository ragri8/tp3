using System;
using System.Collections.Generic;
using System.Linq;

namespace Map {
    public class GameGrid {
        private List<Gridbox> gridboxes;
        public int sizeX;
        public int sizeZ;

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
    }
}
