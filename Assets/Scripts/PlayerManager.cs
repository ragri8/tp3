using System;
using UnityEngine;

using System.Collections.Generic;

namespace Com.MyCompany.MyGame {
    public class PlayerManager : MonoBehaviour {
        
        bool IsFiring;
        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;
        [Tooltip("The Player's UI GameObject Prefab")]
        [SerializeField]
        public GameObject PlayerUiPrefab;
        private GameManager game;
        public GameObject healthbar;
        public float playerSpeed;
        private int longeur = 15;
        private int largeur = 10;
        public float health=10;//vie maximale
        private static float MAX_SPEED = 150.0f;
        private static float ACCEL = 100f;

        private Dictionary<string, KeyCode> controlKeys = new Dictionary<string, KeyCode>();
        private float rotationSensibility;


        void Awake() {
            PlayerManager.LocalPlayerInstance = this.gameObject;
            game = GameObject.Find("Game Manager").GetComponent<GameManager>();
            healthbar = Instantiate(this.healthbar);
            healthbar.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);

            if (tag.Equals("Player")) {
                GameObject _uiGo = Instantiate(this.PlayerUiPrefab); //creation et lien avec le UI pour afficher le nom du joueur
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
                
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
                foreach (GameObject enemy in enemies) {
                        enemy.SendMessage("syncPlayer"); // s'assure que les bots sont synchronisé avec le joueur
                }
            }
        }
        
        void Start() {
            CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();

            playerSpeed = 0;
            controlKeys.Add("Up1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Up1","W")));
            controlKeys.Add("Down1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Down1","S")));
            controlKeys.Add("Left1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left1","A")));
            controlKeys.Add("Right1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right1","D")));
            controlKeys.Add("Slow1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Slow1","LeftShift")));
            controlKeys.Add("Fire1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Fire1","Space")));
            rotationSensibility = PlayerPrefs.GetInt("Sensibility1", 100);
            
            if (_cameraWork != null) {
                 _cameraWork.OnStartFollowing();
            }
        }
    
        void Update() {
            if (!game.paused) {
                ProcessInputs ();
                checkcollision();
            }
        }
        
        public void hit() {
            health--;
            if (health <= 0) {
                //game.SendMessage("gameOver");//on dit au jeu qu'on a perdu quand on meurt
                game.SendMessage("gameOver", LocalPlayerInstance, SendMessageOptions.RequireReceiver);
                Destroy(this.gameObject);
            }
        }

        void ProcessInputs() {
            if (Input.GetKey(controlKeys["Up1"])) {
                if (playerSpeed < MAX_SPEED) {
                    playerSpeed += ACCEL * Time.deltaTime;
                }
            } else if (Input.GetKey(controlKeys["Down1"])) {
                if (playerSpeed > -(MAX_SPEED)) {
                    playerSpeed -= ACCEL * Time.deltaTime;
                }
            } else if (Input.GetKey(controlKeys["Slow1"])) {
                if (playerSpeed > 0) {
                    playerSpeed = Math.Max(playerSpeed - ACCEL * Time.deltaTime, 0);
                } else if (playerSpeed < 0) {
                    playerSpeed = Math.Min(playerSpeed + ACCEL * Time.deltaTime, 0);
                }
            }
            LocalPlayerInstance.transform.Translate(0, 0, playerSpeed * Time.deltaTime);
            if (Input.GetKey(controlKeys["Right1"])) {
                LocalPlayerInstance.transform.Rotate(0,rotationSensibility * Time.deltaTime,0);
            }
            if (Input.GetKey(controlKeys["Left1"])) {
                LocalPlayerInstance.transform.Rotate(0,-rotationSensibility * Time.deltaTime,0);
            }
            
            if (Input.GetKeyDown(controlKeys["Fire1"])) {
                var laser = new Laser();
                Instantiate(laser, transform.position + 20 * transform.forward, transform.rotation);
            }
        }

        public void checkcollision() {
            GameObject[] lasers= GameObject.FindGameObjectsWithTag("Laser");
            foreach (GameObject obj in lasers) {
                Laser laser = obj.GetComponent<Laser>();
                Vector3 poslocal = transform.InverseTransformPoint(obj.transform.position);
                if (poslocal.x < largeur && poslocal.x > -largeur && poslocal.z < longeur && poslocal.z > -longeur) {
                    hit();
                    laser.hit();
                }
            }
        }
    }
}
