using UnityEngine;
using random = UnityEngine.Random;

namespace Map {
    public class ProceduralMapGenerator : MonoBehaviour {
        
        public GameObject groundPrefab;
        public int sizeX = 30;
        public int sizeZ = 30;
        
        private static int groundprefabSize = 10;
        private GameGrid gameGrid;
        private int seed;

        public ProceduralMapGenerator(int seed) {
            this.seed = seed;
            random.InitState(seed);
        }
        public GameObject generateMap() {
            gameGrid = generateGameGrid();
            var map = new GameObject("map");
            generateGround(map);
            generateWalls(map, gameGrid);
            return map;
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
            var position = new Vector3((float)(sizeX * groundprefabSize / 2.0), 0, (float)(sizeZ * groundprefabSize / 2.0));
            Instantiate(groundPrefab, position, Quaternion.identity, map.transform);
        }

        private void generateWalls(GameObject map, GameGrid gameGrid) {
            //foreach (var box in gameGrid.getGridBoxes()) {
            //    generateWall(map, box);
            //}
        }

        private void generateWall(GameObject map, float posX, float posY, float orientation) {
            
        }

        private GameGrid generateGameGrid() {
            return new GameGrid();
        }
    }
}