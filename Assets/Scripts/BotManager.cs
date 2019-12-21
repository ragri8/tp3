using System.Collections;
using AI;
using UnityEngine;

public class BotManager : MonoBehaviour {
    private static float MAX_SPEED = 7.0f;
    private static float ROTATION_SPEED = 50.0f;
    
    private GameObject LocalPlayerInstance;
    private AIBehaviour aiBehaviour;
    public GameManager game;
    private Animator anim;
    private float _speed;
    public bool dead = false;
    public int health;
    private bool isAttacking = false;
    private const float BLOOD_DISTANCE_FROM_BODY = 1.0f;
    
    void Awake() {
        anim = GetComponent<Animator>();
        game = GameObject.Find("Game Manager").GetComponent<GameManager>();
        LocalPlayerInstance = gameObject;
        aiBehaviour = new AIBehaviour(gameObject);
        health = 2;
    }
    
    void Update() {
        if (!game.paused && !dead) {
            aiBehaviour.update();
            ProcessInputs();
        }
    }

    private void OnCollisionEnter(Collision collide) {
        if (collide.gameObject.CompareTag(Global.BULLET_TAG)) {
            hit();
        } else if (collide.gameObject.CompareTag(Global.PLAYER_TAG)) {
            isAttacking = true;
        }
    }

    public void hit() {
        health--;
        var position = transform.position + Vector3.up * BLOOD_DISTANCE_FROM_BODY;
        game.generateBlood(position);
        if (health > 0) {
            anim.SetBool("damage", true);
        } else {
            StartCoroutine(Death());
        }
    }

    IEnumerator Death() {
        anim.SetBool("death", true);
        dead = true;
        LocalPlayerInstance.GetComponent<CapsuleCollider>().enabled = false;
        yield return new WaitForSeconds(5);
        Destroy();
    }

    public void Destroy() {
        Destroy(LocalPlayerInstance);
        game.enemyDestroyed();
    }

    public void removePlayer(GameObject player) {
        aiBehaviour.removePlayer(player);
    }
    
    void ProcessInputs() {
        if (isAttacking) {
            anim.SetBool("attack", true);
            isAttacking = false;
            return;
        }
        
        if (aiBehaviour.getTranslationState() == MovementTranslationState.FORWARD) {
            _speed = MAX_SPEED;
        } else if (aiBehaviour.getTranslationState() == MovementTranslationState.SLOW) {
            _speed = MAX_SPEED;
        } else {
            _speed = 0.0f;
        }
        anim.SetFloat("speed", _speed);

        if (aiBehaviour.getRotationState() == MovementRotationState.LEFT) {
            transform.Rotate(0, ROTATION_SPEED * Time.deltaTime * 1, 0);
        } else if (aiBehaviour.getRotationState() == MovementRotationState.RIGHT) {
            transform.Rotate(0, ROTATION_SPEED * Time.deltaTime * -1, 0);
        }
    }
}