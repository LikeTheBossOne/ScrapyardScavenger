using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used to make an energy drink
 */
[CreateAssetMenu(fileName = "New Resource", menuName = "Resources/SugarPill")]
public class SugarPill : Resource
{

    public SugarPill(int id) : base(id, "SugarPill", "Description here", null)
    {
        // do nothing
    }
}
