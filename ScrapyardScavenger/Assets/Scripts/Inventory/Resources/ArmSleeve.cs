using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Used to make leather armor
 */
[CreateAssetMenu(fileName = "New Resource", menuName = "Resources/ArmSleeve")]
public class ArmSleeve : Resource
{

    public ArmSleeve(int id) : base(id, "ArmSleeve", "Description here", null)
    {
        // do nothing
    }
}
