using System;
using UnityEngine; 
using UnityEngine.SceneManagement; 
using UnityEngine.UI; 
 
/**
 * script pour la gestion de la scene lobby ou l'on attend les joueurs avant de commencer la parte
 */
public class Lobby : MonoBehaviour { 
    public GameObject launch;//bouton pour lencer la partie
    public Text start;//pour l'affichage du decors
    private bool isStarting=false;
    private float tstart=0;//temps du debut du decompte
    // Start is called before the first frame update 
    void Start() {
        start.enabled = false;
    } 
    
    void Update() {
        if (isStarting) {
            start.text = (3 - (int)Math.Floor(Time.time - tstart)).ToString();
            if (start.text.Equals("0")) {
                start.text = "Go!";
                isStarting = false;
                // PhotonNetwork.LoadLevel("Game");//on charge la scene du jeu
                SceneManager.LoadScene("Game");
            }
        }
    }
    public void OnLeftRoom() { 
        SceneManager.LoadScene("Launcher"); //on retourne dans la scene d'acceuil
    }
    
    public void clickLaunch()
    {
        isStarting = true;//on indique que l'on va commencer la partie
        tstart = Time.time;//on prend le temps d'origine du decompte
        start.enabled = true;//on affiche le decompte
        
    }
} 
