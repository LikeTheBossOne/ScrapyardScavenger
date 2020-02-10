using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used to make assault rifle, shotgun, metal rod, 
 * chainmail armor, and metal armor
 */
[CreateAssetMenu(fileName = "New Resource", menuName = "Resources/MetalBox")]
public class MetalBox : Resource
{

    public MetalBox(int id)
    {
        this.id = id;
        name = "MetalBox";
        description = "Metal box description here";
        icon = null;
    }
}
