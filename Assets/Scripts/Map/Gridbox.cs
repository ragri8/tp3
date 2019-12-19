using System;
using System.Collections.Generic;

namespace Map {
    public struct Gridbox {
        private int id;
        private int x;
        private int z;
        private List<int> connectedBox;

        public Gridbox(int id, int x, int z) {
            this.id = id;
            this.x = x;
            this.z = z;
            connectedBox = new List<int>();
        }

        public bool isConnected(int index) {
            return connectedBox.Contains(index);
        }

        public void addConnection(int index) {
            connectedBox.Add(index);
        }
    }
}
