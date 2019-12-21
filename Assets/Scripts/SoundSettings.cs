using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    private int musicVolume;
    private int ambianceVolume;
    private int sfxVolume;
    
    public Slider musicVolumeSlider;
    public Slider ambianceVolumeSlider;
    public Slider sfxVolumeSlider;
    // Start is called before the first frame update
    void Start()
    {
        musicVolume = PlayerPrefs.GetInt("musicVolume",100);
        ambianceVolume = PlayerPrefs.GetInt("ambianceVolume",100);
        sfxVolume = PlayerPrefs.GetInt("sfxVolume",100);
        
        musicVolumeSlider.value = musicVolume;
        ambianceVolumeSlider.value = ambianceVolume;
        sfxVolumeSlider.value = sfxVolume;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeMusicValue(Slider slider) {
        musicVolume = (int)slider.value;
        PlayerPrefs.SetInt("musicVolume", musicVolume);
        PlayerPrefs.Save();
    }
    public void ChangeAmbianceValue(Slider slider) {
        ambianceVolume = (int)slider.value;
        PlayerPrefs.SetInt("ambianceVolume", ambianceVolume);
        PlayerPrefs.Save();
    }
    public void ChangeSfxValue(Slider slider) {
        sfxVolume = (int)slider.value;
        PlayerPrefs.SetInt("sfxVolume", sfxVolume);
        PlayerPrefs.Save();
    }
}
