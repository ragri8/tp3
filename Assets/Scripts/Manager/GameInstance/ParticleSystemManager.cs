using UnityEngine;


namespace Manager.GameInstance {
    public class ParticleSystemManager : MonoBehaviour {
        private float tdebut = 0;

        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            if (Time.time - tdebut > 10) {
                gameObject.SetActive(false);
            }
        }

        public void activate() {
            gameObject.SetActive(true);
            gameObject.GetComponent<ParticleSystem>().Play();
            tdebut = Time.time;
        }
    }
}
