using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used to make a frag grenade
 */
[CreateAssetMenu(fileName = "New Resource", menuName = "Resources/Gunpowder")]
public class Gunpowder : Resource
{

    public Gunpowder(int id)
    {
        this.id = id;
        name = "Gunpowder";
        description = "Gunpowder description here";
        icon = null;
    }
}
