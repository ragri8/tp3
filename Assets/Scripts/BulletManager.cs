
using System.Collections;
using UnityEngine;

public class BulletManager : MonoBehaviour {
    
    private const float SPEED = 50f;
    public static GameObject LocalInstance;
    [SerializeField] private Rigidbody body;
    [SerializeField] private AudioSource audioSource;
    private GameManager game;
    private const float SOUND_LENGTH = 0.4f;
    
    void Awake() {
        LocalInstance = gameObject;
        game = GameObject.Find("Game Manager").GetComponent<GameManager>();
        body.velocity = LocalInstance.transform.forward * SPEED;
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag(Global.ENEMY_TAG)) {
            if (!collision.gameObject.GetComponent<BotManager>().dead) {
                disactivate();
            }
        } else if (!collision.gameObject.CompareTag(Global.PLAYER_TAG)) {
            audioSource.Play();
            StartCoroutine(destroy());
        }
    }

    IEnumerator destroy() {
        var renderer = LocalInstance.GetComponent<Renderer>();
        renderer.enabled = false;
        body.velocity = Vector3.zero;
        var collider = LocalInstance.GetComponent<Collider>();
        collider.enabled = false;
        
        yield return new WaitForSeconds(SOUND_LENGTH);
        renderer.enabled = true;
        collider.enabled = true;
        disactivate();
    }

    void disactivate() {
        LocalInstance.SetActive(false);
    }
}
