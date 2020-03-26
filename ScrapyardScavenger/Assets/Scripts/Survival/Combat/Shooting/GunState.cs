using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunState : MonoBehaviour
{
    public Gun gunType;
    public int ammoCount;
    public int baseAmmo;
    private AudioSource bullet;
    private AudioSource reload;

    void Awake()
    {
        AudioSource[] audio = GetComponents<AudioSource>();
        baseAmmo = gunType.baseClipSize;
        ammoCount = baseAmmo;
        bullet = audio[0];
        reload = audio[1];
    }

    public void bulletSound()
    {
        bullet.Play();
    }

    public void reloadSound()
    {
        reload.Play();
    }

    public void reloadStop()
    {
        reload.Stop();
    }
}
