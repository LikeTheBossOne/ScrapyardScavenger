using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGun", menuName = "Gun")]
public class Gun : Weapon
{
    public float baseDamage = 30;
    public float headShotMultiplier = 2;
    public float baseRateOfFire = 25;
    public int baseClipSize = 25;
    public float reloadTime = 3;
    public List<GunMods> modifiers;
    public bool isAutomatic = false;

    public bool isShotgun = false;
    public int pelletCount = 8;
    public float range = 0;
}
