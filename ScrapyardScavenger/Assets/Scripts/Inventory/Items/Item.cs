using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used by the player for different effects
 */
[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
public class Item : CraftableObject
{
    // consider having this inherit attributes from Craftable
    public int id;
    public string name;
    public string description;
    public bool showInInventory = true;
    public Sprite icon = null;

    public Item(int id, string name, string description, Sprite icon)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.icon = icon;
    }
}
