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
    [SerializeField]
    private Rigidbody playerBody;
    private int longeur = 15;
    private int largeur = 10;
    public float health=10;//vie maximale
    public float MAX_SPEED = 10.0f;
    private float invincibilityFrames = 0.0f;
    private static float maxInvincibilityFrames = 2.0f;
    private Animator anim;
    private bool shoot;
    private Dictionary<string, KeyCode> controlKeys = new Dictionary<string, KeyCode>();
    private float rotationspeed=130;
    private float rotationSensibility;
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject dirt;

    void Awake()
    {
        anim = GetComponent<Animator>();
        LocalPlayerInstance = gameObject;
        anim.SetBool("Static_b",false);
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
       
    }

    void Update() {
        if (!lobby && !game.paused) {
            ProcessInputs ();
            if (invincibilityFrames > 0) {
                invincibilityFrames -= Time.deltaTime;
            }
        }
        anim.SetBool("Jump_b", shoot);
        anim.SetFloat("Speed_f", playerZSpeed);
        transform.Rotate(0, rotationspeed*Time.deltaTime*playerXSpeed, 0);
        dirt.SetActive(anim.GetCurrentAnimatorStateInfo(0).IsName("Run"));
        
    }

    private void LateUpdate() {
        if (!lobby) {
            var transformPos = transform.position;
            transform.position = new Vector3(transformPos.x, 0, transformPos.z);
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

    public void  Debutjeu() {
        lobby = false;
        game = GameObject.Find("Game Manager").GetComponent<GameManager>();
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
        if (health <= 0) {
            //game.SendMessage("gameOver");//on dit au jeu qu'on a perdu quand on meurt
            //game.SendMessage("gameOver", LocalPlayerInstance, SendMessageOptions.RequireReceiver);
            anim.SetFloat("DeathType_int",1);
            anim.SetBool("Death_b",true);
            game.gameOver(LocalPlayerInstance);
            //Destroy(this.gameObject);
        }
    }

    private void ProcessInputs() {
        playerXSpeed = 0;
        playerZSpeed = 0;
        shoot = false;
        
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
        if (Input.GetKey(controlKeys["Fire1"]))
        {
            shoot = true;
        }
        /*
        if (Input.GetKeyDown(controlKeys["Fire1"])) {
            var laser = new Laser();
            Instantiate(laser, transform.position + 20 * transform.forward, transform.rotation);
        }*/
    }
    
}
