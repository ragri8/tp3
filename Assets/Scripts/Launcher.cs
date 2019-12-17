using UnityEngine;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviour {
    
    bool isConnecting;

    [Tooltip("The Ui Panel to let the user enter name, connect and play")] [SerializeField]
    private GameObject controlPanel;

    [Tooltip("The UI Label to inform the user that the connection is in progress")] [SerializeField]
    private GameObject progressLabel;
    // This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
    string gameVersion = "1";
    
    void Awake()
    {
        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        // PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start() {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }
    
    public void Connect() {
        isConnecting = true;
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);
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
