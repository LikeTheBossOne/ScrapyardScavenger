using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGrenade", menuName = "Grenade")]
public class Grenade : Weapon
{
    public float baseDamage = 100;
    public float baseDetonationTime = 5;
    public float areaOfEffect = 5;
}
