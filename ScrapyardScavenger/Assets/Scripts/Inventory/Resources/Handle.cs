using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used to make a shotgun, wooden bat, and metal rod
 */
[CreateAssetMenu(fileName = "New Resource", menuName = "Resources/Handle")]
public class Handle : Resource
{

    public Handle(int id)
    {
        this.id = id;
        name = "Handle";
        description = "Handle description here";
        icon = null;
    }
}
