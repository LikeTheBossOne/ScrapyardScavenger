using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used to make a wooden bat
 */
[CreateAssetMenu(fileName = "New Resource", menuName = "Resources/WoodenPlank")]
public class WoodenPlank : Resource
{

    public WoodenPlank(int id)
    {
        this.id = id;
        name = "WoodenPlank";
        description = "Wooden plank description here";
        icon = null;
    }
}
