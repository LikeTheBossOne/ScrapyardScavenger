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
        reload.pitch /= gunType.reloadTime / reload.clip.length;
    }

    public void bulletSound()
    {
        bullet.PlayOneShot(bullet.clip);
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
