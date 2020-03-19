using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class InGameDataManager : MonoBehaviourPun
{
    public PlayerSceneManager sceneManager;

    // Resources and counts are indexed based on Resource's ID
    [SerializeField]
    public Resource[] resources = null;
    [SerializeField]
    public int[] resourceCounts = null;
    private int resourceIndex;

	public Weapon[] currentWeapons = new Weapon[4];
	public Item currentItem;
	public Armor currentArmor;

    public bool isOpen;
	public bool refreshInv = false;

	// Array of inventory image slots
	private GameObject[] slots;

	// Variables necessary for swapping inventory views
	private Resource[] currentView;
	private Resource[] backpackPart1 = new Resource[8];
	private Resource[] backpackPart2 = new Resource[8];
	private bool firstView;

    void Start()
    {
        sceneManager = GetComponent<PlayerSceneManager>();

        isOpen = false;

        resourceIndex = 0;

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

        if (Input.GetKeyDown(KeyCode.H))
        {
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
				GetComponent<BaseDataManager>().AddResourceToStorage(r, resourceCounts[(int)r.type]);
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

		backpackPart1 = new Resource[8];
		backpackPart2 = new Resource[8];
		currentView = backpackPart1;
		firstView = true;
        resourceIndex = 0;

        isOpen = false;
    }

	int compareObjNames(GameObject first, GameObject second) {
		return first.transform.name.CompareTo(second.transform.name);
	}
}
