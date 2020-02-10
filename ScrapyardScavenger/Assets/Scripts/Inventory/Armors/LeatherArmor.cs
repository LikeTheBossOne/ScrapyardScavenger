using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Equipped by the player to provide a small boost to
 * maximum health
 */
[CreateAssetMenu(fileName = "New Armor", menuName = "Armors/LeatherArmor")]
public class LeatherArmor : Armor
{
    public LeatherArmor(int id) : base(id, "LeatherArmor", "Leather Armor description here", null)
    {
        // do nothing
    }
}
