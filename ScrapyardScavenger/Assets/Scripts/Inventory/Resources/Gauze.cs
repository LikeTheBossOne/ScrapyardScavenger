using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used to make a medpack
 */
[CreateAssetMenu(fileName = "New Resource", menuName = "Resources/Gauze")]
public class Gauze : Resource
{

    public Gauze(int id) : base(id, "Gauze", "Description here", null)
    {
        // do nothing
    }
}
