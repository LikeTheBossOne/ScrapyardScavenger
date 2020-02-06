using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "Inventory/Resource")]
public class Resource : ScriptableObject
{
    public int id;
    public string name;
    public string description;
    public bool showInInventory = true;
    public Sprite icon = null;

    public Resource(int id, string name, string description, Sprite icon)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.icon = icon;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Called when the resource is used in crafting or dropped in the inventory
    public void Use()
    {
        // Resource is removed from the Inventory
    }
}
