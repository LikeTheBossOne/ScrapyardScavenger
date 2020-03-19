using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviourPun
{
    public PlayerSceneManager sceneManager;

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
	public bool refreshInv = false;

	private GameObject[] slots;

	private Resource[] currentView;
	private Resource[] backpackPart1 = new Resource[8];
	private Resource[] backpackPart2 = new Resource[8];
	private bool firstView;

	public GameObject controller;

    void Start()
    {
        sceneManager = GetComponent<PlayerSceneManager>();

        isOpen = false;

        resourceIndex = 0;
        itemIndex = 0;
        weaponIndex = 0;
        armorIndex = 0;

        invSet = 0;

		currentView = backpackPart1;
		firstView = true;
    }

    void Update()
    {
        if (!photonView.IsMine) return;
		if (sceneManager.isInHomeBase) return;
			
		if (!refreshInv) {
			RefreshInventoryView();
		}

        // Check if player is opening/closing inventory
        if (Input.GetKeyDown(KeyCode.I))
        {
			firstView = !firstView;
			foreach (GameObject slot in slots) {
				slot.GetComponent<Image>().sprite = null;
				Color slotColor = slot.GetComponent<Image>().color;
				slotColor.a = 0.0f;
				slot.GetComponent<Image>().color = slotColor;
				slot.transform.GetChild(0).GetComponent<Text>().text = "";
			}
			RefreshInventoryView();
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
        {
			AddResourceToInventory(ResourceType.Gauze);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
			AddResourceToInventory(ResourceType.Disinfectant);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            // use the selected item!
            if (itemCounts[itemIndex] > 0)
            {
                items[itemIndex].Use(this);
            }
            else
            {
                Debug.Log("Could not use item");
            }
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            // make an energy drink!
            itemCounts[(int)ItemType.EnergyDrink]++;
            Debug.Log("Made an energy drink");
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            // print resources?
            PrintResources();
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

    public void PrintItems()
    {
        Debug.Log("Printing all items");
        foreach (CraftableObject craft in items)
        {
            Debug.Log(craft.name);
        }
    }

    public void PrintResources()
    {
        Debug.Log("Printing all resources");
        for (int i = 0; i < resourceCounts.Length; i++)
        {
            if (resourceCounts[i] > 0)
            {
                Debug.Log(resources[i].name + ": " + resourceCounts[i]);
            }
        }
    }

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
        }
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

        Debug.Log($"Crafted {craft.name}");
        return true;
    }

    private int mod(int x, int m)
    {
        return (x % m + m) % m;
    }

	public void AddResourceToInventory(ResourceType type) {
		if (!photonView.IsMine) return;
		resourceCounts[(int)type]++;

		// Add to backpack variables
		if (Array.IndexOf(backpackPart1, resources[(int)type]) < 0 && Array.IndexOf(backpackPart2, resources[(int)type]) < 0) {
			if (resourceIndex <= 7) {
				backpackPart1[resourceIndex] = resources[(int)type];
			} else {
				backpackPart2[resourceIndex - 8] = resources[(int)type];
			}
			resourceIndex++;
		}

		RefreshInventoryView();

		Debug.Log("Adding a " + type.ToString());
	}

	public void RefreshInventoryView() {
		slots = GameObject.FindGameObjectsWithTag("Slot");
		Array.Sort(slots, compareObjNames);

		if (firstView) {
			currentView = backpackPart1;
		} else {
			currentView = backpackPart2;
		}

		foreach(Resource r in currentView) {
			if (r == null || resourceCounts[(int)r.type] <= 0) {
				continue;
			}
			foreach (GameObject slot in slots) {
				if (slot.GetComponent<Image>().sprite == null || slot.GetComponent<Image>().sprite == r.icon){
					slot.GetComponent<Image>().sprite = r.icon;

					Color slotColor = slot.GetComponent<Image>().color;
					slotColor.a = 1.0f;
					slot.GetComponent<Image>().color = slotColor;

					slot.transform.GetChild(0).GetComponent<Text>().text = resourceCounts[(int)r.id].ToString();
					break;
				}
			}
        }
		refreshInv = true;
	}

	public void TransferToStorage()
	{
		foreach (Resource r in resources) {
			if (resourceCounts[(int)r.type] > 0) {
				GetComponent<EquipmentManager>().AddResourceToStorage(r, resourceCounts[(int)r.type]);
			}
		}
		ClearOnLeave();
	}

	public void ClearOnLeave()
	{
		Debug.Log("Clearing resources");
		resourceCounts = new int[(int)ResourceType.SIZE];
		foreach (GameObject slot in slots) {
			slot.GetComponent<Image>().sprite = null;
			Color slotColor = slot.GetComponent<Image>().color;
			slotColor.a = 0.0f;
			slot.GetComponent<Image>().color = slotColor;
			slot.transform.GetChild(0).GetComponent<Text>().text = "";
		}
		backpackPart1 = new Resource[8];
		backpackPart2 = new Resource[8];
		currentView = backpackPart1;
		firstView = true;
		resourceIndex = 0;
		isOpen = false;
	}

	public void ClearOnDeath()
    {
		Debug.Log("Clearing resources");
        resourceCounts = new int[(int)ResourceType.SIZE];
		foreach (GameObject slot in slots) {
			slot.GetComponent<Image>().sprite = null;
			Color slotColor = slot.GetComponent<Image>().color;
			slotColor.a = 0.0f;
			slot.GetComponent<Image>().color = slotColor;
			slot.transform.GetChild(0).GetComponent<Text>().text = "";
		}
        itemCounts = new int[(int)ItemType.SIZE];
        weaponCounts = new int[(int)WeaponType.SIZE];
        armorCounts = new int[(int)ArmorType.SIZE];

		backpackPart1 = new Resource[8];
		backpackPart2 = new Resource[8];
		currentView = backpackPart1;
		firstView = true;
        resourceIndex = 0;
        itemIndex = 0;
        weaponIndex = 0;
        armorIndex = 0;

        invSet = 0;

        isOpen = false;
    }

	int compareObjNames(GameObject first, GameObject second) {
		return first.transform.name.CompareTo(second.transform.name);
	}
}
