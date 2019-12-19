using System;
using Com.MyCompany.MyGame;
using Map;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;


/// <summary>
/// Game manager.
/// Instantiate Player
/// Deals with quiting the room and the game
/// Deals with level loading (outside the in room synchronization)
/// Deal with enemy waves
/// </summary>
public class GameManager : MonoBehaviour {
	
	[Tooltip("The prefab to use for representing the player")] [SerializeField]
	private GameObject playerPrefab;
	//[SerializeField] private GameObject iaPrefab;
	private GameGrid gameGrid;
	
	public GameObject pausepanel;
	public GameObject gameOverPanel;
	public Text vague;
	public bool paused = false;
	public int nbvague = 0;
	
	public int seed = 42;
	public ProceduralMapGenerator mapGenerator;

	void Start() {
		mapGenerator.generateMap();
		gameGrid = mapGenerator.getGrid();
		if (playerPrefab == null) {
			// #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.
			Debug.LogError(
				"<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'",
				this);
		} else {
			if (PlayerManager.LocalPlayerInstance == null) {
				Debug.Log("We are Instantiating player");
				Instantiate(
					playerPrefab,
					new Vector3(
						Random.Range(0, gameGrid.sizeX-1) * ProceduralMapGenerator.gridBoxSize + ProceduralMapGenerator.gridBoxSize / 2.0f,
						2.5f, // haf the size of playerPrefab
						Random.Range(0, gameGrid.sizeZ-1) * ProceduralMapGenerator.gridBoxSize + ProceduralMapGenerator.gridBoxSize / 2.0f),
					Quaternion.identity);
				
			} else {
				Debug.Log("Ignoring scene load");
			}
		}
		loadPlayerAvatar();
	}
	
	void Update() {
		pausepanel.SetActive(paused);//affichage au non du panel de la pause et du numero de la vague
		vague.text = "Wave " + nbvague;
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (paused) {
				Continue(); //on demande au gameManager de tout les joueur d'effectuer la fonction Continue ou Pause
			} else {
				Pause();
			}
		}
		//if (GameObject.FindGameObjectsWithTag("enemy").Length == 0) {
		//	spawnVague();
		//}
	}

	public void QuitApplication() {
		// Application.Quit();
		SceneManager.LoadScene("Launcher");
		
	}

	public void Pause() {
		paused = true;
	}

	public void Continue() {
		paused = false;
	}

	private void gameOver(GameObject player) {
		gameOverPanel.SetActive(true);//on affiche juste le panel de gameOver
	}

	private void spawnVague() {
		nbvague++;

		Debug.Log("vague" + nbvague);
		/*int nbenemy = (int) ((Math.Pow(nbvague, 1.5) + nbvague) / 2);
		for (int i = 0; i < nbenemy; i++) {
			Instantiate(iaPrefab, new Vector3(Random.Range(-400, 400), 0, Random.Range(-300, 300)), Quaternion.identity);
		}*/
	}

	private void loadPlayerAvatar() {
        
	}
}