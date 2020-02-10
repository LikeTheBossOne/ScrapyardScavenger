using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used to make a frag grenade
 */
[CreateAssetMenu(fileName = "New Resource", menuName = "Resources/SafetyPin")]
public class SafetyPin : Resource
{

    public SafetyPin(int id)
    {
        this.id = id;
        name = "SafetyPin";
        description = "Safety pin description here";
        icon = null;
    }
}
