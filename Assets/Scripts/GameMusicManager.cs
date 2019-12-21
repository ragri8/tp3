using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusicManager : MonoBehaviour
{
    public AudioSource mainMusic;

    public AudioSource voley;
    // Start is called before the first frame update
    void Start()
    {
        mainMusic.volume = (1/100f) * PlayerPrefs.GetInt("musicVolume", 100);
        voley.volume = (1/100f) * PlayerPrefs.GetInt("ambianceVolume", 100);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
