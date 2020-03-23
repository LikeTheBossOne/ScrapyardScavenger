using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Sound : MonoBehaviour
{
    public AudioSource sound;

    public void SetVolume(float slider)
    {
        sound.volume = slider;
    }
}
