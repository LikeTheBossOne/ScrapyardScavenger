using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attribute : ScriptableObject
{

    //public string Description;
    public int amount;
    // public Sprite icon;

    public Attribute(int amount)
    {
        this.amount = amount;
    }
}
