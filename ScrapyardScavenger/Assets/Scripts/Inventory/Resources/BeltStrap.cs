﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used to make leather, chainmail, and metal armors
 */

[CreateAssetMenu(fileName = "New Resource", menuName = "Resources/BeltStrap")]
public class BeltStrap : Resource
{

    public BeltStrap(int id) : base(id, "BeltStrap", "Description here", null)
    {
        // do nothing
    }
}
