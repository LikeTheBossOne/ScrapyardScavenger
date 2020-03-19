using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ResourceAmount
{
    public Resource item;
    [Range(1, 999)]
    public int amount;
}

[CreateAssetMenu(fileName = "New Crafting Recipe", menuName = "Crafting/CraftingRecipe")]
public class CraftingRecipe : ScriptableObject
{
    // resources that are needed in order to perform the craft
    public List<ResourceAmount> resources;
}
