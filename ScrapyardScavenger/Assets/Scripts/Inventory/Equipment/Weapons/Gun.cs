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
    public List<GunMods> modifiers;
    public bool isAutomatic = false;
}
