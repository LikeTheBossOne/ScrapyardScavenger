using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used to make leather, chainmail, and metal armors
 */

[CreateAssetMenu(fileName = "New Resource", menuName = "Resources/BeltStrap")]
public class BeltStrap : Resource
{

    public BeltStrap(int id)
    {
        this.id = id;
        name = "BeltStrap";
        description = "Belt strap here";
        icon = null;
    }
}
