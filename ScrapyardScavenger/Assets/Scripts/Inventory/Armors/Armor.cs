using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Equipped to the player before the match starts in order to
 * increase maximum health?
 */
public class Armor : CraftableObject
{
    public int id;
    public string name;
    public string description;
    public Sprite icon = null;

    public Armor(int id, string name, string description, Sprite icon)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.icon = icon;
    }
}
