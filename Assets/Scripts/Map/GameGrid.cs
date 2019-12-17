using System;
using System.Collections.Generic;

namespace Map {
    public class GameGrid {
        private List<Gridbox> gridboxes;
        private int sizeX;
        private int sizeY;

        public GameGrid(int sizeX = 30, int sizeY = 30) {
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            gridboxes = new List<Gridbox>();
            for (var x = 0; x < sizeX; x++) {
                for (var y = 0; y < sizeY; y++) {
                    var gridBox = new Gridbox(new Guid(), x, y);
                    gridboxes.Add(gridBox);
                }
            }
        }

        public bool boxExist(int posX, int posY) {
            return 0 <= posX && posX < sizeX && 0 <= posY && posY < sizeY;
        }
        
        //public List<Tuple<int, int>> getWallsPosition() {
        //    return 
        //}
    }
}
