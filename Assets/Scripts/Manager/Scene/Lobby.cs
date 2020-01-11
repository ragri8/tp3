using System;
using UnityEngine; 
using UnityEngine.SceneManagement; 
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace Manager.Scene {
    public class Lobby : MonoBehaviour {
        public GameObject player;
        [SerializeField] private InputField seedField;

        void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Application.Quit();
            }
        }

        public void OnLeftRoom() {
            SceneManager.LoadScene("Launcher"); //on retourne dans la scene d'acceuil
        }

        public void clickLaunch() {
            setSeed();
            DontDestroyOnLoad(player);
            SceneManager.LoadScene("Game");
        }

        private void setSeed() {
            try {
                var seedString = seedField.text;
                if (string.IsNullOrEmpty(seedString)) {
                    var randomSeed = Random.Range(0, int.MaxValue);
                    Debug.Log("Random seed " + randomSeed + " generated");
                    PlayerPrefs.SetInt("Seed1", randomSeed);
                    PlayerPrefs.Save();
                    return;
                }

                var seed = int.Parse(seedString);
                Debug.Log("Seed " + seed + " generated");
                PlayerPrefs.SetInt("Seed1", seed);
                PlayerPrefs.Save();
            } catch (Exception e) {
                Debug.Log("Invalid seed value");
            }
        }
    }
}
