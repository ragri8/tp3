using System.Collections;
using Manager.Scene;
using UnityEngine;


namespace Manager.GameInstance {
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
            audioSource.volume = (1 / 100f) * PlayerPrefs.GetInt("sfxVolume", 100);
        }

        public void resetVelocity() {
            body.velocity = LocalInstance.transform.forward * SPEED;
        }

        void OnCollisionEnter(Collision collision) {
            if (collision.gameObject.CompareTag(Global.ENEMY_TAG)) {
                if (!collision.gameObject.GetComponent<BotManager>().dead) {
                    disactivate();
                }
            } else if (!collision.gameObject.CompareTag(Global.PLAYER_TAG)) {
                audioSource.Play();
                var position = transform.position + transform.forward * -1;
                var rotation = transform.rotation;
                game.generateSparkle(position, rotation);
                StartCoroutine(destroy());
            }
        }

        IEnumerator destroy() {
            body.velocity = Vector3.zero;
            var position = transform.position;
            transform.position = new Vector3(position.x, -1, position.z);
            var collider = LocalInstance.GetComponent<Collider>();
            collider.enabled = false;

            yield return new WaitForSeconds(SOUND_LENGTH);
            collider.enabled = true;
            disactivate();
        }

        void disactivate() {
            LocalInstance.SetActive(false);
        }
    }
}