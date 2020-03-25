using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Sound : MonoBehaviour
{
    public AudioSource sound;
    public Slider theSlider;
    public Settings playerSetting;

    void Start()
    {
        playerSetting = GameObject.FindWithTag("Settings").GetComponent<Settings>();
        sound.volume = playerSetting.getHomebaseVolume();
        theSlider.value = playerSetting.getHomebaseVolume();
    }

    public void SetVolume(float slider)
    {
        sound.volume = slider;
        playerSetting.setHomebaseVolume(slider);
    }
}
