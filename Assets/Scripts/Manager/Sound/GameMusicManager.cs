using System.Collections;
using Manager.Scene;
using UnityEngine;


namespace Manager.Sound {
    public class GameMusicManager : MonoBehaviour {
        private GameManager game;

        public AudioSource mainMusic;
        public AudioSource intenseMusic;
        public AudioSource voley;
        public AudioSource pauseMusic;
        public AudioSource gameOverMusic;

        private bool lowHealth = false;
        private bool gameOverMark = false;
        private Coroutine musicPausing;
        private Coroutine musicResuming;
        private Coroutine musicIntensifying;

        private int volume;

        // Start is called before the first frame update
        void Start() {
            volume = PlayerPrefs.GetInt("musicVolume", 100);
            game = GameObject.Find("Game Manager").GetComponent<GameManager>();
            mainMusic.volume = (1 / 100f) * PlayerPrefs.GetInt("musicVolume", 100);
            gameOverMusic.volume = (1 / 100f) * PlayerPrefs.GetInt("musicVolume", 100);
            voley.volume = (1 / 100f) * PlayerPrefs.GetInt("ambianceVolume", 100);
        }

        // Update is called once per frame
        void Update() {
            intenseMusic.timeSamples = mainMusic.timeSamples;
            if (Input.GetKeyDown(KeyCode.Escape)) {
                if (game.paused && !game.isGameOver) {
                    Continue(); //on demande au gameManager de tout les joueur d'effectuer la fonction Continue ou Pause
                } else {
                    Pause();
                }
            }
        }

        public void Pause() {
            if (musicResuming != null) {
                StopCoroutine(musicResuming);
            }

            if (lowHealth == true) {
                StopCoroutine(musicIntensifying);
                musicPausing = StartCoroutine(crossfadeMusic(intenseMusic, pauseMusic, 1f, volume));
            } else {
                musicPausing = StartCoroutine(crossfadeMusic(mainMusic, pauseMusic, 1f, volume));
            }

            voley.Stop();
        }

        public void Continue() {
            if (musicPausing != null) {
                StopCoroutine(musicPausing);
            }

            if (lowHealth == true) {
                musicResuming = StartCoroutine(crossfadeMusic(pauseMusic, intenseMusic, 1f, volume));
            } else {
                musicResuming = StartCoroutine(crossfadeMusic(pauseMusic, mainMusic, 1f, volume));
            }

            voley.Play();
        }

        public void lowHealthMusic() {
            if (!lowHealth) {
                musicIntensifying = StartCoroutine(crossfadeMusic(mainMusic, intenseMusic, 1.5f, volume));
                lowHealth = true;
            }
        }

        public void gameOver() {
            if (gameOverMark == false) {
                mainMusic.Stop();
                intenseMusic.Stop();
                pauseMusic.Stop();
                voley.Stop();
                gameOverMusic.Play();
            }

            gameOverMark = true;
        }

        private IEnumerator crossfadeMusic(
            AudioSource firstMusic, AudioSource secondMusic, float duration, float volume) {
            secondMusic.volume = 0f;
            firstMusic.volume = volume;
            float startVolumeFirstMusic = firstMusic.volume;
            if (firstMusic == null || secondMusic == null) {
                yield return null;
            } else {
                while (firstMusic.volume > 0f && secondMusic.volume < volume) {
                    firstMusic.volume -= startVolumeFirstMusic * Time.deltaTime / duration;
                    secondMusic.volume += volume * Time.deltaTime / (duration * 10);
                    yield return null;
                }
            }
        }
    }
}
