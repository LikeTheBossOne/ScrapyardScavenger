using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used by the player for different effects
 */
[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
public abstract class Item : CraftableObject
{
    // consider having this inherit attributes from Craftable
    public int id;
    public string name;
    public string description;
    public bool showInInventory = true;
    public Sprite icon = null;

    /*public Item(int id)
    {
        this.id = id;
    }*/
}
