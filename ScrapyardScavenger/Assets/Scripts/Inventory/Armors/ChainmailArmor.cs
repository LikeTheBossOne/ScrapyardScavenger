using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Equipped by the player to provide a decent boost to
 * maximum health
 */
[CreateAssetMenu(fileName = "New Armor", menuName = "Armors/ChainmailArmor")]
public class ChainmailArmor : Armor
{
    public ChainmailArmor(int id) : base(id, "ChainmailArmor", "Chainmail Armor description here", null)
    {
        // do nothing
    }
}
