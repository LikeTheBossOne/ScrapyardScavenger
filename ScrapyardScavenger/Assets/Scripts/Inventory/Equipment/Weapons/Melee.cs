using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMelee", menuName = "Melee")]
public class Melee : Weapon
{
    public float baseDamage = 40;
    public float baseSwingSpeed = 30;
    public List<MeleeMods> modifiers;
}
