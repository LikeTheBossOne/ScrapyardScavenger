using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

<<<<<<< HEAD
public class InventoryManager : MonoBehaviourPunCallbacks
=======
public class InventoryManager : MonoBehaviourPun
>>>>>>> sprint2-dev
{
    // Resources and counts are indexed based on Resource's ID
    [SerializeField]
    public Resource[] resources = null;
    [SerializeField]
    public int[] resourceCounts = null;
    private int resourceIndex;

    // Crafts and counts are indexed based on the CraftableObject's ID
    [SerializeField]
    public Item[] items = null;
    [SerializeField]
    public int[] itemCounts = null;
    private int itemIndex;

    // Crafts and counts are indexed based on the CraftableObject's ID
    [SerializeField]
    public Weapon[] weapons = null;
    [SerializeField]
    public int[] weaponCounts = null;
    private int weaponIndex;

    // Crafts and counts are indexed based on the CraftableObject's ID
    [SerializeField]
    public Armor[] armors = null;
    [SerializeField]
    public int[] armorCounts = null;
    private int armorIndex;

    // InvSet corresponds to whether we are going through items (0), weapons (1), or armors (2)
    private int invSet;

    public bool isOpen;

    void Start()
    {
        isOpen = false;

        resourceIndex = 0;
        itemIndex = 0;
        weaponIndex = 0;
        armorIndex = 0;

        invSet = 0;
    }

    void Update()
    {
        if (!photonView.IsMine) return;
            

        // Check if player is opening/closing inventory
        if (Input.GetKeyDown(KeyCode.I))
        {
            isOpen = !isOpen;
            if (isOpen)
                Debug.Log("Opened Inventory");
            else
                Debug.Log("Closed Inventory");
        }
            


        if (isOpen)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                invSet += 1;
                invSet = mod(invSet, 3);

                if (invSet == 0)
                    Debug.Log("Switched to Items Inventory");
                if (invSet == 1)
                    Debug.Log("Switched to Weapons Inventory");
                if (invSet == 2)
                    Debug.Log("Switched to Armor Inventory");
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                invSet -= 1;
                invSet = mod(invSet, 3);

                if (invSet == 0)
                    Debug.Log("Switched to Items Inventory");
                if (invSet == 1)
                    Debug.Log("Switched to Weapons Inventory");
                if (invSet == 2)
                    Debug.Log("Switched to Armor Inventory");
            }

            short changeSlot = 0;
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                changeSlot = 1;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                changeSlot = -1;
            }

<<<<<<< HEAD
    void Update()
    {
        // If not me, don't update!
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.I))
=======
            if (changeSlot != 0)
            {
                switch (invSet)
                {
                    case 0:
                        itemIndex += changeSlot;
                        itemIndex = mod(itemIndex, items.Length);
                        Debug.Log($"Item switched to {items[itemIndex].name}");
                        break;
                    case 1:
                        weaponIndex += changeSlot;
                        weaponIndex = mod(weaponIndex, weapons.Length);
                        Debug.Log($"Weapon switched to {weapons[weaponIndex].name}");
                        break;
                    case 2:
                        armorIndex += changeSlot;
                        armorIndex = mod(armorIndex, armors.Length);
                        Debug.Log($"Armor switched to {armors[armorIndex].name}");
                        break;
                    default:
                        break;
                }
            }

            // Craft current item
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (invSet == 0)
                    Craft(items[itemIndex]);
                if (invSet == 1)
                    Craft(weapons[weaponIndex]);
                if (invSet == 2)
                    Craft(armors[armorIndex]);
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
>>>>>>> sprint2-dev
        {
            resourceCounts[(int)ResourceType.Gauze]++;
            Debug.Log("Adding a Gauze");
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            resourceCounts[(int)ResourceType.Disinfectant]++;
            Debug.Log("Adding a Disinfectant");
<<<<<<< HEAD
            Disinfectant d = Resources.Load<Disinfectant>("Resources/Disinfectant");
            AddResource(d);
            Debug.Log("Disinfectant count: " + ResourceCount(d));
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            // craft a medpack
            Debug.Log("Attempting to craft a medpack");
            medpackRecipe.Craft(this);
            PrintCrafts();
            PrintResources();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            // use a medpack!
            // so find a craft that is a medpack
            bool found = false;
            for (int i = 0; i < crafts.Count; i++)
            {
                if (crafts[i].name == "Medpack")
                {
                    // then call .Use(this) on it
                    Debug.Log("Using a medpack!");
                    found = true;
                    ((Medpack) crafts[i]).Use(this);
                    break;
                }
            }
            if (!found)
            {
                Debug.Log("No medpack found so could not use....");
            }
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            // use an energy drink!

            // first make an energy drink for testing purposes
            Debug.Log("Adding an Energy Drink");
            EnergyDrink ed = Resources.Load<EnergyDrink>("Items/Energy Drink");
            crafts.Add(ed);
            Debug.Log("Craft count: " + crafts.Count);
            //Debug.Log("EnergyD count: " + ResourceCount(g));

            // so find a craft that is an energy drink
            bool foundED = false;
            for (int i = 0; i < crafts.Count; i++)
            {
                Debug.Log("name: " + crafts[i].name);
                if (crafts[i].name == "Energy Drink")
                {
                    // then call .Use(this) on it
                    Debug.Log("Using an energy drink!");
                    foundED = true;
                    ((EnergyDrink) crafts[i]).Use(this);
                    break;
                }
            }
            if (!foundED)
            {
                Debug.Log("No energy drink found so could not use....");
            }
        }
    }

    public bool AddResource(Resource resource)
    {
        if (IsFull())
        {
            return false;
=======
>>>>>>> sprint2-dev
        }
        
    }

    public int ResourceCount(Resource resource)
    {
        int count = 0;
        foreach (var res in resources)
        {
            if (res.name == resource.name)
            {
                count++;
            }
        }
        return count;
    }

    public bool ContainsResource(Resource resource)
    {
        return (resourceCounts[resource.id] > 0);
    }

    public void PrintCrafts()
    {
        Debug.Log("Printing all crafted objects");
        foreach (CraftableObject craft in items)
        {
            Debug.Log(craft.name);
        }
    }

    public void PrintResources()
    {
        Debug.Log("Printing all resources");
        foreach (Resource res in resources)
        {
            Debug.Log(res.name);
        }
    }

<<<<<<< HEAD
    public bool RemoveCraft(CraftableObject craft)
    {
        crafts.Remove(craft);
        return true;
    }

=======
    public bool Craft(CraftableObject craft)
    {
        var recipeMaterials = craft.recipe.resources;

        bool doCrafting = true;
        foreach (var resource in recipeMaterials)
        {
            int resourceCount = resource.amount;
            if (resourceCounts[resource.item.id] < resourceCount)
            {
                doCrafting = false;
                break;
            }

        }

        if (!doCrafting)
        {
            Debug.Log($"Can't Craft {craft.name}");
            return false;
        }

        foreach (var resource in recipeMaterials)
        {
            int resourceCount = resource.amount;
            resourceCounts[resource.item.id] -= resourceCount;

            if (craft is Item)
            {
                itemCounts[craft.id]++;
            }
            else if (craft is Weapon)
            {
                weaponCounts[craft.id]++;
            }
            else if (craft is Armor)
            {
                armorCounts[craft.id]++;
            }
        }

        Debug.Log($"Crafted {craft.name}");
        return true;
    }

    private int mod(int x, int m)
    {
        return (x % m + m) % m;
    }
>>>>>>> sprint2-dev
}
