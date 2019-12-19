using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using random = UnityEngine.Random;

namespace Map {
    public class ProceduralMapGenerator : MonoBehaviour {
        
        public GameObject groundPrefab;
        public GameObject wallPrefab; // default for south, rotate for other direction
        public int sizeX = 30;
        public int sizeZ = 30;

        private static int SOUTH_WALL_ORIENTATION = 0;
        private static int NORTH_WALL_ORIENTATION = 180;
        private static int EAST_WALL_ORIENTATION = 90;
        private static int WEST_WALL_ORIENTATION = 270;
        
        public static int gridBoxSize = 10;
        private GameGrid gameGrid;
        private int seed;
        private List<int> activeBoxes;
        private List<int> boxIds;
        private int idsRemaining;

        public ProceduralMapGenerator(int seed) {
            this.seed = seed;
        }
        public GameObject generateMap() {
            reset();
            gameGrid = new GameGrid();
            buildMaze();
            var map = new GameObject("map");
            generateGround(map);
            generateWalls(map, gameGrid);
            return map;
        }

        /**
         * build a mazelike pattern out of the grid
         * use path merging algorithm
         */
        private void buildMaze() {
            // set all grid active at initialization
            activeBoxes = new List<int>(Enumerable.Range(0, gameGrid.getSize()));
            idsRemaining = gameGrid.getSize();
            boxIds = new List<int>(Enumerable.Range(0, gameGrid.getSize()));
            while (idsRemaining > 1) {
                // choose a random index
                var activeBoxIndex = random.Range(0, activeBoxes.Count - 1);
                var gridIndex = activeBoxes[activeBoxIndex];
                if (!mergeAround(gridIndex)) {
                    activeBoxes.RemoveAt(activeBoxIndex);
                } else {
                    if (!mergeAround(gridIndex)) {
                        activeBoxes.RemoveAt(activeBoxIndex);
                    }
                }
            }
        }

        /**
         * try to merge a box with any other one around it
         * choose a random one, check if ids are different and if so merge
         * otherwise, continue untill merged or running out of direction
         * return true if merge is successful, false otherwise
         */
        private bool mergeAround(int index) {
            var directions = new List<int>();
            if (index / sizeX > 0) {
                directions.Add(index - sizeX); // add south
            }
            if (index / sizeX < sizeX * (sizeZ - 1)) {
                directions.Add(index + sizeX); // add north
            }
            if (index % sizeX != sizeX - 1) {
                directions.Add(index + 1); // add east
            }
            if (index % sizeX != 0) {
                directions.Add(index - 1); // add west
            }
            while (directions.Count > 0) {
                var directionIndex = random.Range(0, directions.Count - 1);
                var connectedIndex = directions[directionIndex];
                if (0 <= connectedIndex && connectedIndex < sizeX * sizeZ) {
                    if (boxIds[index] != boxIds[connectedIndex]) {
                        mergeIds(boxIds[index], boxIds[connectedIndex]);
                        gameGrid.linkBoxes(index, connectedIndex);
                        return true;
                    }
                }
                directions.RemoveAt(directionIndex);
            }
            return false;
        }

        /**
         * change all boxids with source value to dest value
         * decrement idsRemaining
         */
        private void mergeIds(int source, int dest) {
            for (var index = 0; index < boxIds.Count; index++) {
                if (boxIds[index] == source) {
                    boxIds[index] = dest;
                }
            }
            idsRemaining -= 1;
        }

        public GameGrid getGrid() {
            if (gameGrid != null) {
                return gameGrid;
            }
            Debug.LogError(
                "<Color=Red><b>Missing</b></Color> Instantiation error. Grid has not been instantiated",
                this);
            return new GameGrid(0, 0);
        }

        public void reset() {
            random.InitState(seed);
        }

        private void generateGround(GameObject map) {
            groundPrefab.transform.localScale = new Vector3(sizeX, 1, sizeZ);
            var position = new Vector3((float)(sizeX * gridBoxSize / 2.0), 0, (float)(sizeZ * gridBoxSize / 2.0));
            Instantiate(groundPrefab, position, Quaternion.identity, map.transform);
        }

        private void generateWalls(GameObject map, GameGrid gameGrid) {
            for (var index = 0; index < gameGrid.getSize(); index++) {
                var connections = gameGrid.connectedInDirections(index);
                var coord = gameGrid.indexToCoord(index);
                // south
                if (!connections.Item1) {
                    generateWall(
                        map,
                        coord.Item1 * gridBoxSize + gridBoxSize / 2.0f,
                        coord.Item2 * gridBoxSize,
                        SOUTH_WALL_ORIENTATION);
                    
                }
                // north
                if (!connections.Item2) {
                    generateWall(
                        map,
                        coord.Item1 * gridBoxSize + gridBoxSize / 2.0f,
                        coord.Item2 * gridBoxSize + gridBoxSize,
                        NORTH_WALL_ORIENTATION);
                }
                // east
                if (!connections.Item3) {
                    generateWall(
                        map,
                        coord.Item1 * gridBoxSize + gridBoxSize,
                        coord.Item2 * gridBoxSize + gridBoxSize / 2.0f,
                        EAST_WALL_ORIENTATION);
                    
                }
                // west
                if (!connections.Item4) {
                    generateWall(
                        map,
                        coord.Item1 * gridBoxSize,
                        coord.Item2 * gridBoxSize + gridBoxSize / 2.0f,
                        WEST_WALL_ORIENTATION);
                }
            }
        }

        private void generateWall(GameObject map, float posX, float posZ, float orientation) {
            var position = new Vector3(posX, gridBoxSize / 2.0f, posZ);
            var orientationVec = Quaternion.Euler(new Vector3(90, 0, orientation));
            Instantiate(wallPrefab, position, orientationVec, map.transform);
        }
    }
}