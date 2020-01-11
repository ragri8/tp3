using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Manager.Scene {
    public class Launcher : MonoBehaviour {
        public Text highscoreText;
        
        void Awake() {
            highscoreText.text = "High score: " + PlayerPrefs.GetInt("Highscore1", 0) + " kills";
        }

        public void GoToLobby() {
            SceneManager.LoadScene("Lobby");
        }

        //fonction pour aller dans la scene de modification des setting (appel via bouton)
        public void GoToSettings() {
            SceneManager.LoadScene("Settings");
        }

        //pour quitter l'application (appel via bouton)
        public void QuitApplication() {
            Application.Quit();
        }
    }
}
