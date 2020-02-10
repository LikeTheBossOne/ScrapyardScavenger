using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used to make an assault rifle, and shotgun
 */
[CreateAssetMenu(fileName = "New Resource", menuName = "Resources/GunBarrel")]
public class GunBarrel : Resource
{

    public GunBarrel(int id)
    {
        this.id = id;
        name = "GunBarrel";
        description = "Gun barrel description here";
        icon = null;
    }
}
