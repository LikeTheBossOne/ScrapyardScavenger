using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used to make a medpack
 */
[CreateAssetMenu(fileName = "New Resource", menuName = "Resources/Gauze")]
public class Gauze : Resource
{

    public Gauze(int id)
    {
        this.id = id;
        name = "Gauze";
        description = "Gauze description here";
        icon = null;
    }
}
