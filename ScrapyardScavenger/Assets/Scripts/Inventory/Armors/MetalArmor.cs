using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Equipped by the player to provide a large boost to
 * maximum health
 */
[CreateAssetMenu(fileName = "New Armor", menuName = "Armors/MetalArmor")]
public class MetalArmor : Armor
{
    public MetalArmor(int id) : base(id, "MetalArmor", "Metal Armor description here", null)
    {
        // do nothing
    }
}
