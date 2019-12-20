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
    
    void Awake() {
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

    private void OnCollisionEnter(Collision collide) {
        if (collide.gameObject.CompareTag(Global.BULLET_TAG)) {
            hit();
        }
    }

    public void hit() {
        hp--;
        if (hp > 0) {
            anim.SetBool("damage", true);
        } else {
            Death();
        }
    }

    IEnumerator Death() {
        // TODO call death animation
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
        game.enemyDestroyed();
    }

    public IEnumerator Destroy() {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
        game.enemyDestroyed();
    }

    public void removePlayer(GameObject player) {
        aiBehaviour.removePlayer(player);
    }
    
    void ProcessInputs() {
        var timelapse = Time.deltaTime;
        
        if (aiBehaviour.getTranslationState() == MovementTranslationState.FORWARD) {
            _speed = MAX_SPEED;
        } else if (aiBehaviour.getTranslationState() == MovementTranslationState.HALF_FORWARD) {
            _speed = MAX_SPEED / 2.0f;
        } else if (aiBehaviour.getTranslationState() == MovementTranslationState.SLOW) {
            _speed = 0.0f;
        } else {
            _speed = 0.0f;
        }
        anim.SetFloat("speed",_speed);
        if (_speed > 0.0f) {
            var angle = transform.eulerAngles.y;
            var speedX = (float) (_speed * Math.Sin(Util.toRad(angle)));
            var speedZ = (float) (_speed * Math.Cos(Util.toRad(angle)));
            //var velocity = new Vector3(speedX, 0, speedZ);
            //body.velocity = velocity;
        }
        
        anim.SetFloat("angularspeed",ROTATION_SPEED);
        if (aiBehaviour.getRotationState() == MovementRotationState.LEFT) {
            
            //LocalPlayerInstance.transform.Rotate(0,ROTATION_SPEED * timelapse,0);
        } else if (aiBehaviour.getRotationState() == MovementRotationState.RIGHT) {
            //LocalPlayerInstance.transform.Rotate(0,-ROTATION_SPEED * timelapse,0);
        }
    }
}