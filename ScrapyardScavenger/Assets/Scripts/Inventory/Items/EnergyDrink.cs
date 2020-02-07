using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used by the player to increase defense temporarily
 */
[CreateAssetMenu(fileName = "New Item", menuName = "Items/EnergyDrink")]
public class EnergyDrink : Item
{
    public EnergyDrink(int id) : base(id, "EnergyDrink", "Energy drink description here", null)
    {
        // do nothing
    }
}
