using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CraftableObject : ScriptableObject
{
    public int id;
    
    public new string name;
    public string description;
    public bool showInInventory = true;
    public Sprite icon = null;
    public CraftingRecipe recipe;
}
