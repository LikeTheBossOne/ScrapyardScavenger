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

	private string[] slots = new string[] {"slot1", "slot2", "slot3", "slot4", "slot5", "slot6", "slot7", "slot8"}; 
	private string[] slotCounts = new string[] {"slot1Text", "slot2Text", "slot3Text", "slot4Text", "slot5Text", "slot6Text", "slot7Text", "slot8Text"}; 

    void Start()
    {
        sceneManager = GetComponent<PlayerSceneManager>();

        isOpen = false;

        resourceIndex = 0;
        itemIndex = 0;
        weaponIndex = 0;
        armorIndex = 0;

        invSet = 0;

		foreach (Resource r in resources) {
			r.imageSlotName = "";
		}
    }

    void Update()
    {
        if (!photonView.IsMine) return;
        if (sceneManager.isInHomeBase) return;

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
			addResourceToInventory(ResourceType.Gauze);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
			addResourceToInventory(ResourceType.Disinfectant);
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

	public void addResourceToInventory(ResourceType type) {
		resourceCounts[(int)type]++;
		Resource r = resources[(int)type];
		if (r.imageSlotName == "") {
			foreach (string slot in slots) {
				if (GameObject.FindWithTag(slot).GetComponent<Image>().sprite == null || GameObject.FindWithTag(slot).GetComponent<Image>().sprite == r.icon){
					r.imageSlotName = slot;
					GameObject.FindWithTag(slot).GetComponent<Image>().sprite = r.icon;
					Color slotColor = GameObject.FindWithTag(slot).GetComponent<Image>().color;
					slotColor.a = 1.0f;
					GameObject.FindWithTag(slot).GetComponent<Image>().color = slotColor;
					break;
				}
			}
		}
		GameObject.FindWithTag(r.imageSlotName + "Text").GetComponent<Text>().text = resourceCounts[(int)type].ToString();
		Debug.Log("Adding a " + type.ToString());
	}

    public void Clear()
    {
        resourceCounts = new int[(int)ResourceType.SIZE];
        itemCounts = new int[(int)ItemType.SIZE];
        weaponCounts = new int[(int)WeaponType.SIZE];
        armorCounts = new int[(int)ArmorType.SIZE];

        resourceIndex = 0;
        itemIndex = 0;
        weaponIndex = 0;
        armorIndex = 0;

        invSet = 0;

        isOpen = false;
    }
}
