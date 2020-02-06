using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used to make metal armor
 */
[CreateAssetMenu(fileName = "New Resource", menuName = "Resources/ShoulderPlate")]
public class ShoulderPlate : Resource
{

    public ShoulderPlate(int id) : base(id, "ShoulderPlate", "Description here", null)
    {
        // do nothing
    }
}
