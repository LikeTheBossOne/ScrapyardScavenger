﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used to make a frag grenade
 */
[CreateAssetMenu(fileName = "New Resource", menuName = "Resources/SafetyPin")]
public class SafetyPin : Resource
{

    public SafetyPin(int id) : base(id, "SafetyPin", "Description here", null)
    {
        // do nothing
    }
}
