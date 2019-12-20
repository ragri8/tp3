using Map;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


/// <summary>
/// Game manager.
/// Instantiate Player
/// Deals with quiting the room and the game
/// Deals with level loading (outside the in room synchronization)
/// Deal with enemy waves
/// </summary>
public class GameManager : MonoBehaviour {
	
	[Tooltip("The prefab to use for representing the enemy")] [SerializeField]
	private GameObject enemyPrefab;
	//[SerializeField] private GameObject iaPrefab;
	private GameGrid gameGrid;
	private GameObject player;
	public GameObject pausepanel;
	public GameObject gameOverPanel;
	public Text vague;
	public bool paused = false;
	public int nbvague = 0;
	private int nbrEnemies = 0;
	private int maxEnemies = 3;
	private int enemyKilled = 0;
	private float respawnCooldown = 0;
	private float minimumRespawnTime = 3;
	public int minEnemyRangeSpawn = 6;
	public int maxEnemyRangeSpawn = 10;
	public float timeBasedEnemyIncrease = 5;
	public static float timeStartBasedEnemyIncrease = 15;
	
	public int seed = 42;
	public ProceduralMapGenerator mapGenerator;

	void Start() {
		mapGenerator.generateMap();
		gameGrid = mapGenerator.getGrid();
		player=GameObject.Find("player");
		if (player == null) {
			// #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.
			Debug.LogError(
				"<Color=Red><b>Missing</b></Color> objet player introuvable",
				this);
		} else
		{
			player.transform.position = gameGrid.randomPosition(player.transform.GetChild(0).lossyScale.y / 2.0f);
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

		if (!paused) {
			spawninHandler();
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

	private void spawninHandler() {
		timeBasedEnemyIncrease -= Time.deltaTime;
		if (respawnCooldown > 0) {
			respawnCooldown -= Time.deltaTime;
		} else if (nbrEnemies < maxEnemies) {
			generateEnemies(1);
			if (nbrEnemies < maxEnemies) {
				timeBasedEnemyIncrease = 1;
			}
			timeBasedEnemyIncrease = timeStartBasedEnemyIncrease;
		}
		if (timeBasedEnemyIncrease <= 0) {
			maxEnemies++;
			timeBasedEnemyIncrease = timeStartBasedEnemyIncrease;
		}
	}

	private void generateEnemies(int nbr) {
		var playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
		var playerPos = playerTransform.position;
		var playerIndexPos = gameGrid.realWorldCoordToIndex(
			playerPos.x,
			playerPos.z,
			0,
			0);
		for (var i = 0; i < nbr; i++) {
			var enemyPos = gameGrid.randomPositionInRange(
				playerIndexPos,
				minEnemyRangeSpawn,
            	maxEnemyRangeSpawn,
            	playerTransform.GetChild(0).lossyScale.y / 2.0f);
			Instantiate(
            	enemyPrefab,
            	enemyPos,
            	Quaternion.identity);
			nbrEnemies++;
		}
	}

	public void enemyDestroyed() {
		nbrEnemies--;
		enemyKilled++;
		if (respawnCooldown <= 0) {
			respawnCooldown = minimumRespawnTime;
		}
	}
}