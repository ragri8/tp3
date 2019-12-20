
using UnityEngine;

public class BulletManager : MonoBehaviour {
    
    private const float SPEED = 50f;
    public static GameObject LocalInstance;
    [SerializeField] private Rigidbody body;
    
    void Awake() {
        LocalInstance = gameObject;
        body.velocity = LocalInstance.transform.forward * SPEED;
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag(Global.ENEMY_TAG)) {
            if (!collision.gameObject.GetComponent<BotManager>().dead) {
                Destroy(gameObject);
            }
        } else if (!collision.gameObject.CompareTag(Global.PLAYER_TAG)) {
            Destroy(gameObject);
        }
    }
}
