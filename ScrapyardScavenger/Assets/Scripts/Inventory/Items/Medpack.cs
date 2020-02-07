using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used by the player to recover health
 */
[CreateAssetMenu(fileName = "New Item", menuName = "Items/Medpack")]
public class Medpack : Item
{
    public Medpack(int id) : base(id, "Medpack", "Medpack description here", null)
    {
        // do nothing
    }
}
