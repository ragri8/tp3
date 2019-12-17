using System;
using System.Collections.Generic;

namespace Map {
    public struct Gridbox {
        private Guid id;
        private int x;
        private int y;
        private List<Guid> connectedBox;

        public Gridbox(Guid id, int x, int y) {
            this.id = id;
            this.x = x;
            this.y = y;
            connectedBox = new List<Guid>();
        }
    }
}
