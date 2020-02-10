using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used to make gauzes
 */
[CreateAssetMenu(fileName = "New Resource", menuName = "Resources/Disinfectant")]
public class Disinfectant : Resource
{

    public Disinfectant(int id)
    {
        this.id = id;
        name = "Disinfectant";
        description = "Disinfectant here";
        icon = null;
    }
}
