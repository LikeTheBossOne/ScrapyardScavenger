using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Equipped to the player before the match starts in order to
 * increase maximum health?
 */
public class Armor : CraftableObject
{
    public float damageMultiplier;
}

public enum ArmorType
{
    ChainmailArmor = 0,
    LeatherArmor = 0,
    MetalArmor = 0,
    SIZE
}
