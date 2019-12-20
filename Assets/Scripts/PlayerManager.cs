using System;
using UnityEngine;

using System.Collections.Generic;

public class PlayerManager : MonoBehaviour {
    
    bool IsFiring;
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;
    [Tooltip("The Player's UI GameObject Prefab")]
    [SerializeField]
    private GameManager game;
    public GameObject healthbar;
    public float playerSpeed;
    public float playerXSpeed;
    public float playerZSpeed;
    private bool lobby=true;
    private Rigidbody playerBody;
    private int longeur = 15;
    private int largeur = 10;
    public float health=10;//vie maximale
    public float MAX_SPEED = 10.0f;
    private float invincibilityFrames = 0.0f;
    private static float maxInvincibilityFrames = 2.0f;
    private Animator anim;
    private Dictionary<string, KeyCode> controlKeys = new Dictionary<string, KeyCode>();
    private float rotationSensibility;

    void Awake()
    {
        anim = GetComponent<Animator>();
        LocalPlayerInstance = gameObject;
        anim.SetBool("Static_b",false);
        if (tag.Equals("Player")) {

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
            foreach (GameObject enemy in enemies) {
                    enemy.SendMessage("syncPlayer"); // s'assure que les bots sont synchronisé avec le joueur
            }
        }
        playerBody = LocalPlayerInstance.GetComponent<Rigidbody>();
    }
    
    void Start() {
        playerSpeed = 0;
        controlKeys.Add("Up1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Up1","W")));
        controlKeys.Add("Down1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Down1","S")));
        controlKeys.Add("Left1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left1","A")));
        controlKeys.Add("Right1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right1","D")));
        controlKeys.Add("Slow1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Slow1","LeftShift")));
        controlKeys.Add("Fire1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Fire1","Space")));
        rotationSensibility = PlayerPrefs.GetInt("Sensibility1", 100);
        
        //if (_cameraWork != null) {
        //     _cameraWork.OnStartFollowing();
        //}
    }

    void Update() {
        if (!lobby&&!game.paused) {
            ProcessInputs ();
            if (invincibilityFrames > 0) {
                invincibilityFrames -= Time.deltaTime;
            }
        }
    }

    void OnCollisionEnter(Collision collide) {
        if (invincibilityFrames > 0) {
            return;
        }

        if (collide.gameObject.CompareTag(Global.ENEMY_TAG)) {
            hit();
        }
    }

    public void  Debutjeu()
    {
        lobby = false;
        game = GameObject.Find("Game Manager").GetComponent<GameManager>();
        healthbar = Instantiate(this.healthbar);
        healthbar.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);

        CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();
        if (_cameraWork != null) {
            _cameraWork.OnStartFollowing();
        }
    }
    
    private void hit() {
        health--;
        invincibilityFrames = maxInvincibilityFrames;
        if (health <= 0) {
            Destroy(this.gameObject);
            game.gameOver(LocalPlayerInstance);
        }
    }

    void ProcessInputs() {
        playerXSpeed = 0;
        playerZSpeed = 0;
        if (Input.GetKey(controlKeys["Up1"])) {
            playerZSpeed += 1;
        } else if (Input.GetKey(controlKeys["Down1"])) {
            playerZSpeed -= 1;
        } else if (Input.GetKey(controlKeys["Slow1"])) {
            playerSpeed = 0;
        }
        if (Input.GetKey(controlKeys["Right1"])) {
            playerXSpeed += 1;
        }
        if (Input.GetKey(controlKeys["Left1"])) {
            playerXSpeed -= 1;
        }

        var direction = new Vector3(playerXSpeed, 0, playerZSpeed);
        anim.SetFloat("Speed_f",playerZSpeed);
        if (direction != Vector3.zero) {
            //playerBody.velocity = MAX_SPEED * direction; // TODO direction should be normalized
        }
        /*
        if (Input.GetKeyDown(controlKeys["Fire1"])) {
            var laser = new Laser();
            Instantiate(laser, transform.position + 20 * transform.forward, transform.rotation);
        }*/
    }
    
}
