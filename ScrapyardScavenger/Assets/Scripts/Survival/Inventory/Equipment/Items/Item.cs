using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used by the player for different effects
 */
[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
public abstract class Item : Equipment
{
    public abstract void Use(InGameDataManager manager);
}

public enum ItemType
{
    EnergyDrink = 0,
    Medpack,
    SIZE
}