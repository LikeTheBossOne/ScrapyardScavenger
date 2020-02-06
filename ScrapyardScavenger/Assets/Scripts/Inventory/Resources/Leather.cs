﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used to make leather armor and chainmail armor
 */
[CreateAssetMenu(fileName = "New Resource", menuName = "Resources/Leather")]
public class Leather : Resource
{

    public Leather(int id) : base(id, "Leather", "Description here", null)
    {
        // do nothing
    }
}
