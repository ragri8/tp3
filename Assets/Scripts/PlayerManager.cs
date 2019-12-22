using UnityEngine;

using System.Collections.Generic;

public class PlayerManager : MonoBehaviour {
    
    bool IsFiring;
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;
    [Tooltip("The Player's UI GameObject Prefab")]
    [SerializeField]
    private GameManager game;
    private GameMusicManager music;
    public GameObject healthbar;
    public float playerXSpeed;
    public float playerZSpeed;
    private bool lobby = true;
    [SerializeField]
    private Rigidbody playerBody;
    public float health = 10;
    private float invincibilityFrames = 0.0f;
    private static float maxInvincibilityFrames = 2.0f;
    private Animator anim;
    private bool shoot;
    private bool hasShot = false;
    private Dictionary<string, KeyCode> controlKeys = new Dictionary<string, KeyCode>();
    private float rotationSensibility;
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject dirt;
    private const float HEAD_DISTANCE_FROM_BODY = 1.0f;

    void Awake() {
        anim = GetComponent<Animator>();
        LocalPlayerInstance = gameObject;
        anim.SetBool("Static_b",false);
        playerBody = LocalPlayerInstance.GetComponent<Rigidbody>();
    }
    
    void Start() {
        controlKeys.Add("Up1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Up1","W")));
        controlKeys.Add("Down1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Down1","S")));
        controlKeys.Add("Left1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left1","A")));
        controlKeys.Add("Right1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right1","D")));
        controlKeys.Add("Fire1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Fire1","Space")));
        rotationSensibility = PlayerPrefs.GetInt("Sensibility1", 100);
       
    }

    void Update() {
        if (!lobby && !game.paused) {
            ProcessInputs ();
            if (invincibilityFrames > 0) {
                invincibilityFrames -= Time.deltaTime;
            }
        }
    }

    public void Continue() {
        anim.enabled = true;
    }

    public void Pause() {
        anim.enabled = false;
    }

    private void LateUpdate() {
        if (!lobby) {
            var transformPos = transform.position;
            transform.position = new Vector3(transformPos.x, 0, transformPos.z);
        }
    }

    void OnCollisionStay(Collision collision) {
        if (invincibilityFrames > 0) {
            return;
        }

        if (collision.gameObject.CompareTag(Global.ENEMY_TAG)) {
            if (!collision.gameObject.GetComponent<BotManager>().dead) {
                hit();
            }
        }
    }

    public void  Debutjeu() {
        lobby = false;
        game = GameObject.Find("Game Manager").GetComponent<GameManager>();
        music = GameObject.Find("Game Music").GetComponent<GameMusicManager>();
        healthbar = Instantiate(this.healthbar);
        healthbar.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        playerBody.useGravity = true;
        
        var cameraWork = gameObject.GetComponent<CameraWork>();
        if (cameraWork != null) {
            cameraWork.OnStartFollowing();
        }
    }
    
    private void hit() {
        health--;
        invincibilityFrames = maxInvincibilityFrames;
        var position = transform.position + Vector3.up * HEAD_DISTANCE_FROM_BODY;
        game.generateBlood(position);
        if (health <= 0) {
            anim.SetFloat("DeathType_int",1);
            anim.SetBool("Death_b",true);
            game.gameOver(LocalPlayerInstance);
        }
        else if (health <= 3)
        {
            music.lowHealthMusic();
        }
    }

    public void shootBullet() {
        var gunPosition = gun.transform.position;
        var gunPos = gunPosition + gun.transform.forward * 0.35f;
        var rotation = transform.rotation;
        game.generateBullet(gunPos, rotation);
        
        var casingPos = gunPosition + gun.transform.right * 0.5f;
        game.generateCasing(casingPos, rotation);
    }

    private void ProcessInputs() {
        playerXSpeed = 0;
        playerZSpeed = 0;
        shoot = false;
        
        if (Input.GetKey(controlKeys["Up1"])) {
            playerZSpeed += 1;
        } else if (Input.GetKey(controlKeys["Down1"])) {
            playerZSpeed -= 1;
        }
        if (Input.GetKey(controlKeys["Right1"])) {
            playerXSpeed += 1;
        }
        if (Input.GetKey(controlKeys["Left1"])) {
            playerXSpeed -= 1;
        }
        if (Input.GetKey(controlKeys["Fire1"])) {
            shoot = true;
        }
        anim.SetBool("Jump_b", shoot);
        anim.SetFloat("Speed_f", playerZSpeed);
        transform.Rotate(0, rotationSensibility * Time.deltaTime * playerXSpeed, 0);
        dirt.SetActive(anim.GetCurrentAnimatorStateInfo(0).IsName("Run"));
        
        if (anim.GetCurrentAnimatorStateInfo(3).IsName("shoot")) {
            if (!hasShot) {
                hasShot = true;
                shootBullet();
            }
        } else {
            hasShot = false;
        }
    }
}
