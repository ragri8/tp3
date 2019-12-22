using System.Collections.Generic;
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
	[SerializeField] private GameObject blood;
	[SerializeField] private GameObject enemy_blood;
	[SerializeField] private GameObject spakle;
	[SerializeField] private GameObject bullet;
	[SerializeField] private GameObject casing;
	private List<GameObject> bloodlist=new List<GameObject>();
	private List<GameObject> enemy_bloodlist=new List<GameObject>();
	private List<GameObject> spaklelist=new List<GameObject>();
	private List<GameObject> bulletlist=new List<GameObject>();
	private List<GameObject> casinglist=new List<GameObject>();
	public bool isGameOver = false;
	public GameObject pausepanel;
	public GameObject gameOverPanel;
	public Text enemyKilledDisplayText;
	public bool paused = false;
	private int nbrEnemies = 0;
	private int maxEnemies = 10;
	private int enemyKilled = 0;
	private float respawnCooldown = 0;
	private float minimumRespawnTime = 0;
	public int minEnemyRangeSpawn = 4;
	public int maxEnemyRangeSpawn = 5;
	public float timeBasedEnemyIncrease = 5;
	private const float TIME_ENEMY_INCREASE_VALUE = 10;
	private const float ENEMY_Z_POSITION = 0.0f;
	private const float MAX_ENEMY_DISTANCE = 50;
	
	public ProceduralMapGenerator mapGenerator;

	private GameMusicManager music;
	public AudioSource comboJingle;
	
	void Start() {
		for (int i = 0; i < 10; i++) {
			bloodlist.Add(Instantiate(blood, Vector3.zero, blood.transform.rotation));//pour le pooling
			enemy_bloodlist.Add(Instantiate(enemy_blood, Vector3.zero, enemy_blood.transform.rotation));
			bulletlist.Add(Instantiate(bullet, Vector3.zero, Quaternion.identity));
			casinglist.Add(Instantiate(casing, Vector3.zero, Quaternion.identity));
			spaklelist.Add(Instantiate(spakle, Vector3.zero, spakle.transform.rotation));
		}

		var seed = PlayerPrefs.GetInt("Seed1", 42);
		mapGenerator.setSeed(seed);
		map = mapGenerator.generateMap();
		gameGrid = mapGenerator.getGrid();
		player = GameObject.Find("player");
		player.SendMessage("Debutjeu");
		if (player == null) {
			Debug.LogError(
				"<Color=Red><b>Missing</b></Color> objet player introuvable",
				this);
		} else {
			player.transform.position = gameGrid.randomPosition(
				player.transform.GetChild(0).lossyScale.y / 2.0f);
		}
		loadPlayerAvatar();
		music = GameObject.Find("Game Music").GetComponent<GameMusicManager>();
		comboJingle.volume = (1/100f) * PlayerPrefs.GetInt("sfxVolume", 100);
	}
	
	void Update() {
		pausepanel.SetActive(paused);
		enemyKilledDisplayText.text = "Enemies killed: " + enemyKilled;
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (paused) {
				Continue();
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
		music.Continue();
	}

	public void gameOver(GameObject player) {
		saveHighscore();
		gameOverPanel.SetActive(true);//on affiche juste le panel de gameOver
		isGameOver = true;
		music.gameOver();
		var enemies = FindObjectsOfType<BotManager>();
		foreach (BotManager enemy in enemies) {
			enemy.removePlayer(player);
		}
	}

	private void saveHighscore() {
		var oldHighscore = PlayerPrefs.GetInt("Highscore1", 0);
		if (enemyKilled > oldHighscore) {
			PlayerPrefs.SetInt("Highscore1", enemyKilled);
			PlayerPrefs.Save();
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
			teleportEnemies();
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
				ENEMY_Z_POSITION);
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
		if (enemyKilled % 5 == 0) {
			if (comboJingle.pitch < 2) {
				comboJingle.pitch += 0.1f;
			}
			comboJingle.Play();
		}
	}

	private GameObject get_sparkle_Available() {
		foreach (var s in spaklelist) {
			if (!s.activeSelf) {
				return s;
			}
		}
		GameObject newsparkle = Instantiate(
			spakle,
			Vector3.zero,
			Quaternion.identity);
		spaklelist.Add(newsparkle);
		return newsparkle;
	}
	
	private GameObject get_blood_Available() {
		foreach (var b in bloodlist) {
			if (!b.activeSelf) {
				return b;
			}
		}
		GameObject newblood = Instantiate(
			blood,
			Vector3.zero,
			blood.transform.rotation);
		bloodlist.Add(newblood);
		return newblood;
	}	
	
	private GameObject get_enemy_blood_Available() {
		foreach (var eb in enemy_bloodlist) {
			if (!eb.activeSelf) {
				return eb;
			}
		}
		GameObject new_enemy_blood = Instantiate(
			enemy_blood,
			Vector3.zero,
			enemy_blood.transform.rotation);
		enemy_bloodlist.Add(new_enemy_blood);
		return new_enemy_blood;
	}	
	
	private GameObject get_bullet_Available() {
		foreach (var b in bulletlist) {
			if (!b.activeSelf) {
				return b;
			}
		}
		GameObject newbullet = Instantiate(
			bullet,
			Vector3.zero,
			Quaternion.identity);
		bulletlist.Add(newbullet);
		return newbullet;
	}	
	
	private GameObject get_casing_Available() {
		foreach (var c in casinglist) {
			if (!c.activeSelf) {
				return c;
			}
		}
		GameObject newcasing = Instantiate(
			casing,
			Vector3.zero,
			Quaternion.identity);
		casinglist.Add(newcasing);
		return newcasing;
	}
	
	private void deleteAll() {
		var enemies = FindObjectsOfType<BotManager>();
		foreach (var enemy in enemies) {
			enemy.Destroy();
		}
		Destroy(player);
		Destroy(map);
	}

	public void generateSparkle(Vector3 position, Quaternion rotation) {
		var sparkleEffect = get_sparkle_Available();

		var bloodTransform = sparkleEffect.transform;
		bloodTransform.position = position;
		bloodTransform.rotation = rotation;
		
		sparkleEffect.GetComponent<particleSystem>().activate();
	}

	public void generateBlood(Vector3 position) {
		var bloodObject = get_blood_Available();

		var bloodTransform = bloodObject.transform;
		bloodTransform.position = position;
		
		bloodObject.GetComponent<particleSystem>().activate();
	}

	public void generateEnemyBlood(Vector3 position) {
		var bloodObject = get_enemy_blood_Available();

		var bloodTransform = bloodObject.transform;
		bloodTransform.position = position;
		
		bloodObject.GetComponent<particleSystem>().activate();
	}

	public void generateBullet(Vector3 position, Quaternion rotation) {
		var bullet = get_bullet_Available();

		var bulletTransform = bullet.transform;
		bulletTransform.position = position;
		bulletTransform.rotation = rotation;

		bullet.SetActive(true);

		bullet.GetComponent<BulletManager>().resetVelocity();
	}

	public void generateCasing(Vector3 position, Quaternion rotation) {
		var casingObject = get_casing_Available();

		var casingTransform = casingObject.transform;
		casingTransform.position = position;
		casingTransform.rotation = rotation;

		casingObject.SetActive(true);

		casingObject.GetComponent<Casing>().activate();
	}

	public void teleportEnemies() {
		var playerPos = player.transform.position;
		var playerIndexPos = gameGrid.realWorldCoordToIndex(
			playerPos.x,
			playerPos.z,
			0,
			0);
		var enemies = FindObjectsOfType<BotManager>();
		foreach (BotManager enemy in enemies) {
			if (Vector3.Distance(enemy.transform.position, playerPos) > MAX_ENEMY_DISTANCE) {
				enemy.transform.position = gameGrid.randomPositionInRange(
					playerIndexPos,
					minEnemyRangeSpawn,
					minEnemyRangeSpawn,
					ENEMY_Z_POSITION);
			}
		}
	}
}