using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used to make an assault rifle
 */
[CreateAssetMenu(fileName = "New Resource", menuName = "Resources/GunStock")]
public class GunStock : Resource
{

    public GunStock(int id) : base(id, "GunStock", "Description here", null)
    {
        // do nothing
    }
}
