using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ResourceAmount
{
    public Resource item;
    [Range(1, 999)]
    public int amount;
}

/*[Serializable]
public struct CraftAmount
{
    public CraftableObject item;
    [Range(1, 999)]
    public int amount;
}*/

[CreateAssetMenu(fileName = "New Crafting Recipe", menuName = "Crafting/CraftingRecipe")]
public class CraftingRecipe : ScriptableObject
{
    // resources that are needed in order to perform the craft
    public List<ResourceAmount> materials;

    // craftable objects that are created as a result of this craft
    //public List<CraftAmount> results;
    public CraftableObject result;
    public string howdy = "yes";

    public bool CanCraft(InventoryManager inventory)
    {
        foreach (ResourceAmount resourceAmount in materials)
        {
            Debug.Log("Checking resource " + resourceAmount.item.name);
            if (inventory.ResourceCount(resourceAmount.item) < resourceAmount.amount)
            {
                Debug.Log("Cannot craft");
                return false;
            }
        }
        Debug.Log("Can craft");
        return true;
    }

    public void Craft(InventoryManager inventory)
    {
        if (CanCraft(inventory))
        {
            foreach (ResourceAmount resourceAmount in materials)
            {
                for (int i = 0; i < resourceAmount.amount; i++)
                {
                    Debug.Log("Removing " + resourceAmount.item.name);
                    inventory.RemoveResource(resourceAmount.item);
                }
            }

            // add the crafted item
            inventory.AddCraft(result);

        }
    }
    
}
