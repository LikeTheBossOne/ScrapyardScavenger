using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
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

	// Variables for player shoot
	public delegate void EquipmentSwitched();
	public event EquipmentSwitched OnEquipmentSwitched;
	public int currentWeaponIndex;
	private GameObject currentObject;
	public Transform gunParent;
	public Transform meleeParent;
	public Transform grenadeParent;
	public Transform medShotParent;
	public bool isReloading = false;

    public bool isOpen;
	public bool refreshInv = false;

	// Array of inventory image slots
	[SerializeField]
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

        // Keycode check for switching inventory views
        if (Input.GetKeyDown(KeyCode.F))
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

		if (Input.GetKeyDown(KeyCode.Alpha1)
			&& currentWeaponIndex != 0
			&& currentWeapons[0] != null)
			photonView.RPC("EquipWeapon", RpcTarget.All, 0);
		if (Input.GetKeyDown(KeyCode.Alpha2)
			&& currentWeaponIndex != 1
			&& currentWeapons[1] != null)
			photonView.RPC("EquipWeapon", RpcTarget.All, 1);
		/* No longer valid
		if (Input.GetKeyDown(KeyCode.Alpha3)
			&& currentWeaponIndex != 2
			&& currentWeapons[2] != null)
			photonView.RPC("EquipWeapon", RpcTarget.All, 2);
		*/
		if (Input.GetKeyDown(KeyCode.Alpha4)
			&& currentWeaponIndex != 3
			&& currentWeapons[3] != null)
			photonView.RPC("EquipWeapon", RpcTarget.All, 3);

		if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown("joystick button 1")) {
			currentItem.Use(this);
			GetComponent<BaseDataManager>().itemCounts[GetComponent<BaseDataManager>().equipment[4].id]--;
			GetComponent<BaseDataManager>().equipment[4] = null;
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

	[PunRPC]
	void EquipWeapon(int index)
	{
		if (currentObject != null)
			currentObject.SetActive(false);


		Transform parent;
		if (index == 0) parent = gunParent;
		else if (index == 1) parent = gunParent;
		else if (index == 2) parent = meleeParent;
		else if (index == 3) parent = grenadeParent;
		else if (index == 4) parent = medShotParent;
		else return;

		currentObject = parent.Equals(gunParent) ? parent.GetChild(index).gameObject : parent.GetChild(0).gameObject;

		currentObject.SetActive(true);
		currentWeaponIndex = index;

		OnEquipmentSwitched?.Invoke();
	}

    public Equipment getCurrentEquipment() {
		if (currentWeaponIndex == -1) return null;
		return currentWeapons[currentWeaponIndex];
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
		Debug.Log("Transferring to storage");
		// First bring each resource back
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

		currentWeapons = new Weapon[4];
		currentItem = null;
		currentArmor = null;

		isOpen = false;
	}

	public void ClearOnDeath()
    {
		Debug.Log("Clearing resources");
        resourceCounts = new int[(int)ResourceType.SIZE];
		try {
			foreach (GameObject slot in slots) {
				slot.GetComponent<Image>().sprite = null;
				Color slotColor = slot.GetComponent<Image>().color;
				slotColor.a = 0.0f;
				slot.GetComponent<Image>().color = slotColor;
				slot.transform.GetChild(0).GetComponent<Text>().text = "";
			}
		} catch (Exception e) {
			// If something happens... oh well
		}

		backpackPart1 = new Resource[8];
		backpackPart2 = new Resource[8];
		currentView = backpackPart1;
		firstView = true;
        resourceIndex = 0;

		currentWeapons = new Weapon[4];
		currentItem = null;
		currentArmor = null;

        isOpen = false;
    }

	int compareObjNames(GameObject first, GameObject second) {
		return first.transform.name.CompareTo(second.transform.name);
	}

    public void grenadeThrown()
    {
        currentObject = null;
		GetComponent<BaseDataManager>().weaponCounts[GetComponent<BaseDataManager>().equipment[3].id]--;
		GetComponent<BaseDataManager>().equipment[3] = null;
		currentWeapons[3] = null;
        EquipWeapon(0);
    }
}
