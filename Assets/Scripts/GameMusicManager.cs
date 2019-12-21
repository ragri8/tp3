using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusicManager : MonoBehaviour
{
    private GameManager game;
    
    public AudioSource mainMusic;
    public AudioSource voley;
    public AudioSource pauseMusic;
    public AudioSource jingle;

    private Coroutine musicPausing;
    private Coroutine musicResuming;

    private int volume;
    // Start is called before the first frame update
    void Start()
    {
        game = GameObject.Find("Game Manager").GetComponent<GameManager>();
        mainMusic.volume = (1/100f) * PlayerPrefs.GetInt("musicVolume", 100);
        voley.volume = (1/100f) * PlayerPrefs.GetInt("ambianceVolume", 100);
        pauseMusic.volume = (1/100f) * PlayerPrefs.GetInt("musicVolume", 100);
        jingle.volume = (1/100f) * PlayerPrefs.GetInt("musicVolume", 100);
        volume = PlayerPrefs.GetInt("musicVolume", 100);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (game.paused) {
                Continue(); //on demande au gameManager de tout les joueur d'effectuer la fonction Continue ou Pause
            } else {
                Pause();
            }
        }
    }
    
    public void Pause() {
        if (musicResuming != null)
        {
            StopCoroutine(musicResuming);
        }
        musicPausing = StartCoroutine(crossfadeMusic(mainMusic, pauseMusic, 1f, volume));
        voley.Stop();
    }

    public void Continue() {
        if (musicPausing != null)
        {
            StopCoroutine(musicPausing);
        }
        musicResuming = StartCoroutine(crossfadeMusic(pauseMusic, mainMusic, 1f, volume));
        voley.Play();
    }
    
    private IEnumerator crossfadeMusic(AudioSource firstMusic, AudioSource secondMusic, float duration, float volume)
    {
        secondMusic.volume = 0f;
        firstMusic.volume = volume;
        float startVolumeFirstMusic = firstMusic.volume;
        if (firstMusic == null || secondMusic == null)
        {
            yield return null;
        }
        else
        {
            secondMusic.Play();
            while (firstMusic.volume > 0f && secondMusic.volume < volume)
            {
                firstMusic.volume -= startVolumeFirstMusic * Time.deltaTime / duration;
                secondMusic.volume += volume * Time.deltaTime / (duration * 10);
                yield return null;
            }
            firstMusic.Pause();
        }
    }
}
