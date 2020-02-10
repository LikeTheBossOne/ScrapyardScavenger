using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used to make metal armor
 */
[CreateAssetMenu(fileName = "New Resource", menuName = "Resources/ShoulderPlate")]
public class ShoulderPlate : Resource
{

    public ShoulderPlate(int id)
    {
        this.id = id;
        name = "ShoulderPlate";
        description = "Shoulder plate description here";
        icon = null;
    }
}
