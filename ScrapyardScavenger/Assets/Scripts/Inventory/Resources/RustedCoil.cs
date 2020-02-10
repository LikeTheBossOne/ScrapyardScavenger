using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used to make assault rifle, and shotgun
 */
[CreateAssetMenu(fileName = "New Resource", menuName = "Resources/RustedCoil")]
public class RustedCoil : Resource
{

    public RustedCoil(int id)
    {
        this.id = id;
        name = "RustedCoil";
        description = "Rusted coil description here";
        icon = null;
    }
}
