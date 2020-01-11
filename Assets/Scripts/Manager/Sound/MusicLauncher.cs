using UnityEngine;


namespace Manager.Sound {
    public class MusicLauncher : MonoBehaviour {
        public AudioSource musicLauncher;

        // Start is called before the first frame update
        void Start() {
            musicLauncher.volume = (1 / 100f) * PlayerPrefs.GetInt("musicVolume", 100);
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
