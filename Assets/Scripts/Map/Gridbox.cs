using System;
using System.Collections.Generic;

namespace Map {
    public struct Gridbox {
        private int id;
        private int x;
        private int z;
        private List<int> adjacentSquares;

        public Gridbox(int id, int x, int z) {
            this.id = id;
            this.x = x;
            this.z = z;
            adjacentSquares = new List<int>();
        }

        public bool isConnected(int index) {
            return adjacentSquares.Contains(index);
        }

        public void addConnection(int index) {
            adjacentSquares.Add(index);
        }

        public List<int> getAdjacents() {
            return adjacentSquares;
        }

        public int getAdjacentCount() {
            return adjacentSquares.Count;
        }
    }
}
