using System;
using UnityEngine; 
using UnityEngine.SceneManagement; 
using UnityEngine.UI; 
 
/**
 * script pour la gestion de la scene lobby ou l'on attend les joueurs avant de commencer la parte
 */
public class Lobby : MonoBehaviour { 
  
    
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    public void OnLeftRoom() { 
        SceneManager.LoadScene("Launcher"); //on retourne dans la scene d'acceuil
    }
    
    public void clickLaunch()
    {
        SceneManager.LoadScene("Game");
    }
} 
