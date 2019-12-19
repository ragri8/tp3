using Com.MyCompany.MyGame.AI;
using UnityEngine;

namespace Com.MyCompany.MyGame {
    public class BotManager : MonoBehaviour {
        private static float MAX_SPEED = 120.0f;
        private static float TRANSLATION_ACCELERATION = 30.0f;
        private static float ROTATION_SPEED = 70.0f;
        
        private GameObject LocalPlayerInstance;
        public GameManager game;
        public bool isSharpshooter = true;
        
        private bool IsFiring;
        private float _speed;
        private int longeur=11;
        private int largeur=7;
        private int longeurplayer = 15;
        private int largeurplayer = 10;
        public bool destroy=false;
        private AIBehaviour _aiBehaviour;
        
        void Awake() {
            game = GameObject.Find("Game Manager").GetComponent<GameManager>();
            LocalPlayerInstance = this.gameObject;
            _aiBehaviour = new AIBehaviour(LocalPlayerInstance.transform, isSharpshooter);
        }

        void Start() {}
        
        void Update() {
            if (!game.paused) {
                _aiBehaviour.update();
                ProcessInputs();
                //checkcollision();
            }
        }

        public void checkcollision() {
            GameObject[] lasers= GameObject.FindGameObjectsWithTag("Laser");
            foreach (GameObject obj in lasers) {
                Laser laser = obj.GetComponent<Laser>();
                Vector3 poslocal = transform.InverseTransformPoint(obj.transform.position);
                if (!destroy&&poslocal.x < largeur && poslocal.x > -largeur && poslocal.z < longeur && poslocal.z > -longeur) {
                    destroy = true;
                    laser.hit();
                    hit();
                }
            } 
            GameObject[] players= GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject obj in players) {
                Vector3 poslocal = transform.InverseTransformPoint(obj.transform.position + obj.transform.forward*longeurplayer + obj.transform.right*largeurplayer); 
                if (!!destroy&&poslocal.x < largeur && poslocal.x > -largeur && poslocal.z < longeur && poslocal.z > -longeur) {
                    destroy = true;
                    hit();
                    obj.SendMessage("hit");
                }
                poslocal = transform.InverseTransformPoint(obj.transform.position+obj.transform.forward*longeurplayer-obj.transform.right*largeurplayer); 
                if (!destroy&&poslocal.x < largeur && poslocal.x > -largeur && poslocal.z < longeur && poslocal.z > -longeur) {
                    destroy = true;
                    hit();
                    obj.SendMessage("hit");
                }
                poslocal = transform.InverseTransformPoint(obj.transform.position-obj.transform.forward*longeurplayer+obj.transform.right*largeurplayer); 
                if (!destroy&&poslocal.x < largeur && poslocal.x > -largeur && poslocal.z < longeur && poslocal.z > -longeur) {
                    destroy = true;
                    hit();
                    obj.SendMessage("hit");
                }
                poslocal = transform.InverseTransformPoint(obj.transform.position-obj.transform.forward*longeurplayer-obj.transform.right*largeurplayer); 
                if (!destroy&&poslocal.x < largeur && poslocal.x > -largeur && poslocal.z < longeur && poslocal.z > -longeur) {
                    destroy = true;
                    hit();
                    obj.SendMessage("hit");
                }
            } 
        }
        public void hit() {
            Destroy(this.gameObject);
        }
        
        void ProcessInputs() {
            var timelapse = Time.deltaTime;
            if (_aiBehaviour.getTranslationState() == MovementTranslationState.FORWARD) {
                if (_speed < MAX_SPEED) {
                    _speed += TRANSLATION_ACCELERATION * timelapse;
                }
                
            } else if (_aiBehaviour.getTranslationState() == MovementTranslationState.HALF_FORWARD) {
                if (_speed < MAX_SPEED / 2) {
                    _speed += TRANSLATION_ACCELERATION * timelapse;
                } else {
                    _speed -= TRANSLATION_ACCELERATION * timelapse;
                }
            } else if (_aiBehaviour.getTranslationState() == MovementTranslationState.SLOW) {
                if (_speed > 0) {
                    _speed -= TRANSLATION_ACCELERATION * timelapse;
                } else {
                    _speed = 0;
                }
            }
            LocalPlayerInstance.transform.Translate(0, 0, _speed * Time.deltaTime);
            if (_aiBehaviour.getRotationState() == MovementRotationState.LEFT) {
                LocalPlayerInstance.transform.Rotate(0,ROTATION_SPEED * Time.deltaTime,0);
            } else if (_aiBehaviour.getRotationState() == MovementRotationState.RIGHT) {
                LocalPlayerInstance.transform.Rotate(0,-ROTATION_SPEED * Time.deltaTime,0);
            }
            if (_aiBehaviour.isFiring) {
                _aiBehaviour.isFiring = false;
                Instantiate(new Laser(), transform.position + 20 * transform.forward, transform.rotation);
            }
        }
    }
}