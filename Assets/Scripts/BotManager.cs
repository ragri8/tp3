using System;
using System.Collections;
using AI;
using UnityEngine;

public class BotManager : MonoBehaviour {
    private static float MAX_SPEED = 7.0f;
    private static float TRANSLATION_ACCELERATION = 30.0f;
    private static float ROTATION_SPEED = 20.0f;
    
    private GameObject LocalPlayerInstance;
    private AIBehaviour aiBehaviour;
    public GameManager game;
    public bool isSharpshooter = true;
    private Animator anim;
    private Transform transform;
    private bool IsFiring;
    private float _speed;
    private int longeur=11;
    private int largeur=7;
    private int longeurplayer = 15;
    private int largeurplayer = 10;
    public bool destroy=false;
    public int hp;
    
    void Awake()
    {
        anim = GetComponent<Animator>();
        game = GameObject.Find("Game Manager").GetComponent<GameManager>();
        LocalPlayerInstance = gameObject;
        transform = LocalPlayerInstance.GetComponent<Transform>();
        aiBehaviour = new AIBehaviour(gameObject);
        hp = 1;
    }
    
    void Update() {
        if (!game.paused) {
            aiBehaviour.update();
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
                hit();
                obj.SendMessage("hit");
            }
            poslocal = transform.InverseTransformPoint(obj.transform.position+obj.transform.forward*longeurplayer-obj.transform.right*largeurplayer); 
            if (!destroy&&poslocal.x < largeur && poslocal.x > -largeur && poslocal.z < longeur && poslocal.z > -longeur) {
                hit();
                obj.SendMessage("hit");
            }
            poslocal = transform.InverseTransformPoint(obj.transform.position-obj.transform.forward*longeurplayer+obj.transform.right*largeurplayer); 
            if (!destroy&&poslocal.x < largeur && poslocal.x > -largeur && poslocal.z < longeur && poslocal.z > -longeur) {
                hit();
                obj.SendMessage("hit");
            }
            poslocal = transform.InverseTransformPoint(obj.transform.position-obj.transform.forward*longeurplayer-obj.transform.right*largeurplayer); 
            if (!destroy&&poslocal.x < largeur && poslocal.x > -largeur && poslocal.z < longeur && poslocal.z > -longeur) {
                hit();
                obj.SendMessage("hit");
            }
        } 
    }
    public void hit()
    {
        hp--;
        if (hp < 1)
        {
            anim.SetBool("damage",true);
        }
        else
        {
            anim.SetBool("death", true);
            destroy = true;
            StartCoroutine(Death());
        }
        
        game.enemyDestroyed();
    }
    IEnumerator Death() 
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
    
    void ProcessInputs() {
        var timelapse = Time.deltaTime;
        
        if (aiBehaviour.getTranslationState() == MovementTranslationState.FORWARD) {
            _speed = MAX_SPEED;
        } else if (aiBehaviour.getTranslationState() == MovementTranslationState.HALF_FORWARD) {
            _speed = MAX_SPEED / 2.0f;
        } else if (aiBehaviour.getTranslationState() == MovementTranslationState.SLOW) {
            _speed = 0.0f;
        }
        anim.SetFloat("speed",_speed);
        if (_speed != 0.0f) {
            var angle = transform.eulerAngles.y;
            var speedX = (float) (_speed * Math.Sin(Util.toRad(angle)));
            var speedZ = (float) (_speed * Math.Cos(Util.toRad(angle)));
            //body.velocity = velocity;
        }
        
        anim.SetFloat("angularspeed",ROTATION_SPEED);
        if (aiBehaviour.getRotationState() == MovementRotationState.LEFT) {
            
            //LocalPlayerInstance.transform.Rotate(0,ROTATION_SPEED * timelapse,0);
        } else if (aiBehaviour.getRotationState() == MovementRotationState.RIGHT) {
            //LocalPlayerInstance.transform.Rotate(0,-ROTATION_SPEED * timelapse,0);
        }

        /*
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
        }*/
    }
}