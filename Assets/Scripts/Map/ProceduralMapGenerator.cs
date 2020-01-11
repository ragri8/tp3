using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using random = UnityEngine.Random;
using Random = System.Random;

namespace Map {
    public class ProceduralMapGenerator : MonoBehaviour {

        public GameObject groundPrefab;
        public GameObject wallPrefab; // default for south, rotate for other direction
        public GameObject wallBloodyPrefab; // go away written on the wall
        public GameObject cornerPrefab; // default for south-east, rotate for other direction
        public int sizeX = 30;
        public int sizeZ = 30;

        private static int SOUTH_WALL_ORIENTATION = 0;
        private static int NORTH_WALL_ORIENTATION = 180;
        private static int EAST_WALL_ORIENTATION = 270;
        private static int WEST_WALL_ORIENTATION = 90;

        private GameGrid gameGrid;
        private int seed;
        private List<int> activeBoxes;
        private List<int> boxIds;
        private int idsRemaining;
        private List<GameObject> gridBoxObjects;

        public ProceduralMapGenerator(int seed) {
            this.seed = seed;
        }

        public void setSeed(int seed) {
            this.seed = seed;
        }

        public GameObject generateMap() {
            reset();
            gameGrid = new GameGrid(sizeX, sizeZ);
            buildMaze();
            gameGrid.initializeDistances();
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
            var position = new Vector3(
                (float) (sizeX * GameGrid.gridBoxSize / 2.0),
                0,
                (float) (sizeZ * GameGrid.gridBoxSize / 2.0));
            Instantiate(groundPrefab, position, Quaternion.identity, map.transform);
        }

        private void generateWalls(GameObject map, GameGrid gameGrid) {
            for (var index = 0; index < gameGrid.getSize(); index++) {
                var connections = gameGrid.connectedInDirections(index);
                var coord = gameGrid.indexToCoord(index);
                var posX = coord.Item1 * GameGrid.gridBoxSize;
                var posZ = coord.Item2 * GameGrid.gridBoxSize;

                if (!connections.Item1) {
                    generateWall(map, posX, posZ, SOUTH_WALL_ORIENTATION);
                }

                if (!connections.Item2) {
                    generateWall(map, posX, posZ, NORTH_WALL_ORIENTATION);
                }

                if (!connections.Item3) {
                    generateWall(map, posX, posZ, EAST_WALL_ORIENTATION);
                }

                if (!connections.Item4) {
                    generateWall(map, posX, posZ, WEST_WALL_ORIENTATION);
                }

                
                if (connections.Item1) {
                    if (connections.Item3) { // south-east corner
                        generateCorner(map, posX, posZ, SOUTH_WALL_ORIENTATION);
                    }
                    if (connections.Item4) { // south-west corner
                        generateCorner(map, posX, posZ, WEST_WALL_ORIENTATION);
                    }
                }
                if (connections.Item2) {
                    if (connections.Item4) { // north-east corner
                        generateCorner(map, posX, posZ, NORTH_WALL_ORIENTATION);
                    }
                    if (connections.Item3) { // north-west corner
                        generateCorner(map, posX, posZ, EAST_WALL_ORIENTATION);
                    }
                }
            }

            generateOuterWalls(map);
        }

        private void generateWall(GameObject map, float posX, float posZ, float orientation) {
            var position = new Vector3(
                posX + GameGrid.gridBoxSize / 2.0f,
                0,
                posZ + GameGrid.gridBoxSize / 2.0f);

            var orientationVec = Quaternion.Euler(new Vector3(0, orientation, 0));
            Instantiate(getWallPrefab(), position, orientationVec, map.transform);
        }

        private void generateCorner(GameObject map, float posX, float posZ, float orientation) {
            var position = new Vector3(
                posX + GameGrid.gridBoxSize / 2.0f,
                0,
                posZ + GameGrid.gridBoxSize / 2.0f);

            var orientationVec = Quaternion.Euler(new Vector3(0, orientation, 0));
            Instantiate(cornerPrefab, position, orientationVec, map.transform);
        }
        private void generateOuterWalls(GameObject map) {
            for (var i = 0; i < sizeX; i++) {
                var posX = i * GameGrid.gridBoxSize;
                var posZSouth = - GameGrid.gridBoxSize;
                var posZNorth = sizeZ * GameGrid.gridBoxSize;
                generateWall(map, posX, posZSouth, NORTH_WALL_ORIENTATION);
                generateWall(map, posX, posZNorth, SOUTH_WALL_ORIENTATION);
            }
            for (var i = 0; i < sizeZ; i++) {
                var posXWest = - GameGrid.gridBoxSize;
                var posXEast = sizeX * GameGrid.gridBoxSize;
                var posZ = i * GameGrid.gridBoxSize;
                generateWall(map, posXWest, posZ, EAST_WALL_ORIENTATION);
                generateWall(map, posXEast, posZ, WEST_WALL_ORIENTATION);
            }
            generateCorner(map, -GameGrid.gridBoxSize, sizeZ * GameGrid.gridBoxSize, SOUTH_WALL_ORIENTATION); // north-west corner
            generateCorner(map, sizeX * GameGrid.gridBoxSize, sizeZ * GameGrid.gridBoxSize, WEST_WALL_ORIENTATION); // north-east corner
            generateCorner(map, sizeX * GameGrid.gridBoxSize, -GameGrid.gridBoxSize, NORTH_WALL_ORIENTATION); // south-east corner
            generateCorner(map, -GameGrid.gridBoxSize, -GameGrid.gridBoxSize, EAST_WALL_ORIENTATION); // south-west corner
        }
    
        private GameObject getWallPrefab() {
            var randomVal = random.Range(0, 20);
            if (randomVal == 0) {
                return wallBloodyPrefab;
            }
            return wallPrefab;
        }
    }
}
