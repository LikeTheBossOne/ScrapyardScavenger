﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used to make a wooden bat
 */
[CreateAssetMenu(fileName = "New Resource", menuName = "Resources/WoodenPlank")]
public class WoodenPlank : Resource
{

    public WoodenPlank(int id) : base(id, "WoodenPlank", "Description here", null)
    {
        // do nothing
    }
}
