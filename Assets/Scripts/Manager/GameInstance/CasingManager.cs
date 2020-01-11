using UnityEngine;

namespace Manager.GameInstance {
    public class CasingManager : MonoBehaviour {
        [SerializeField] private Light light;
        [SerializeField] private AudioSource sound;
        [SerializeField] private AudioSource fire;

        private float tdebut = 0;
        // Start is called before the first frame update

        private void Awake() {
            sound.volume = (1 / 100f) * PlayerPrefs.GetInt("sfxVolume", 100);
            fire.volume = (1 / 100f) * PlayerPrefs.GetInt("sfxVolume", 100);
        }

        void Start() {

        }

        void Update() {
            if (Time.time - tdebut > 0.1) {
                light.enabled = false;
            }

            if (Time.time - tdebut > 5) {
                gameObject.SetActive(false);
            }
        }

        private void OnCollisionEnter(Collision other) {
            if (transform.position.y < 0.5) {
                sound.Play();
            }
        }

        public void activate() {
            gameObject.SetActive(true);
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            light.enabled = true;
            tdebut = Time.time;
            fire.Play();
        }
    }
}
