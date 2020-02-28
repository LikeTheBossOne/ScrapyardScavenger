using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunState : MonoBehaviour
{
    public Gun gunType;
    public int ammoCount;
    public int baseAmmo;

    void Awake()
    {
        baseAmmo = gunType.baseClipSize;
        ammoCount = baseAmmo;
    }
}
