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
	private GameObject map;

	public bool isGameOver = false;
	public GameObject pausepanel;
	public GameObject gameOverPanel;
	public Text enemyKilledDisplayText;
	public bool paused = false;
	private int nbrEnemies = 0;
	private int maxEnemies = 5;
	private int enemyKilled = 0;
	private float respawnCooldown = 0;
	private float minimumRespawnTime = 1;
	public int minEnemyRangeSpawn = 4;
	public int maxEnemyRangeSpawn = 6;
	public float timeBasedEnemyIncrease = 5;
	private const float TIME_ENEMY_INCREASE_VALUE = 10;
	
	public int seed = 42;
	public ProceduralMapGenerator mapGenerator;

	void Start() {
		mapGenerator.setSeed(seed);
		map = mapGenerator.generateMap();
		gameGrid = mapGenerator.getGrid();
		player=GameObject.Find("player");
		player.SendMessage("Debutjeu");
		if (player == null) {
			// #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.
			Debug.LogError(
				"<Color=Red><b>Missing</b></Color> objet player introuvable",
				this);
		} else {
			player.transform.position = gameGrid.randomPosition(player.transform.GetChild(0).lossyScale.y / 2.0f);
		}
		loadPlayerAvatar();
	}
	
	void Update() {
		pausepanel.SetActive(paused);//affichage au non du panel de la pause et du numero de la vague
		enemyKilledDisplayText.text = "Enemies killed: " + enemyKilled;
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (paused) {
				Continue(); //on demande au gameManager de tout les joueur d'effectuer la fonction Continue ou Pause
			} else {
				Pause();
			}
		}

		if (!paused && !isGameOver) {
			spawningHandler();
		}
	}

	public void QuitApplication() {
		SceneManager.LoadScene("Launcher");
		deleteAll();
	}

	public void Pause() {
		paused = true;
	}

	public void Continue() {
		paused = false;
	}

	public void gameOver(GameObject player) {
		gameOverPanel.SetActive(true);//on affiche juste le panel de gameOver
		isGameOver = true;
		var enemies = FindObjectsOfType<BotManager>();
		foreach (BotManager enemy in enemies) {
			enemy.removePlayer(player);
		}
	}

	private void loadPlayerAvatar() {
		var playerManager = FindObjectOfType<PlayerManager>();
		playerManager.Debutjeu();
	}

	private void spawningHandler() {
		timeBasedEnemyIncrease -= Time.deltaTime;
		if (respawnCooldown > 0) {
			respawnCooldown -= Time.deltaTime;
		} else if (nbrEnemies < maxEnemies) {
			generateEnemies(1);
			if (nbrEnemies < maxEnemies) {
				timeBasedEnemyIncrease = 1;
			}
			timeBasedEnemyIncrease = TIME_ENEMY_INCREASE_VALUE;
		}
		if (timeBasedEnemyIncrease <= 0) {
			maxEnemies++;
			timeBasedEnemyIncrease = TIME_ENEMY_INCREASE_VALUE;
		}
	}

	private void generateEnemies(int nbr) {
		var playerTransform = player.transform;
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

	private void deleteAll() {
		var enemies = FindObjectsOfType<BotManager>();
		foreach (BotManager enemy in enemies) {
			enemy.Destroy();
		}
		Destroy(player);
		Destroy(map);
	}

	public void generateBlood(Vector3 position) {
		// todo get ground
		// todo get blood
		
		// todo put in place
		
		// todo set active
		
		//todo awake
	}

	public void generateBullet(Vector3 position, Quaternion rotation) {
		// todo get bullet
		
		// todo put in place
		
		// todo set active
		
		//todo awake
	}

	public void generateCasing(Vector3 position, Quaternion rotation) {
		// todo get casing
		
		// todo put in place
		
		// todo set active
		
		//todo awake
	}
}