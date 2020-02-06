using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used by the player for different effects
 */
[CreateAssetMenu(fileName = "New Item", menuName = "Resources/Item")]
public class Item : ScriptableObject
{
    // consider having this implement Craftable
    // thereby inheriting attributes

    public Item()
    {
        // wait until I decide to implement Craftable
    }
}
