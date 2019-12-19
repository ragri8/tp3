using UnityEngine;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviour {


    void Awake()
    {
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
