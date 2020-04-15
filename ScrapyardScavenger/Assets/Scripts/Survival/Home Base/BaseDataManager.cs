using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class BaseDataManager : MonoBehaviourPunCallbacks
{
    public PlayerSceneManager sceneManager;
    private InGameDataManager inGameManager;

    public Transform gunParent;
    public Transform meleeParent;
    public Transform grenadeParent;
    public Transform medShotParent;

	// Crafts and counts are indexed based on the CraftableObject's ID
	[SerializeField]
	public Item[] items = null;
	[SerializeField]
	public int[] itemCounts = null;

	// Crafts and counts are indexed based on the CraftableObject's ID
	[SerializeField]
	public Weapon[] weapons = null;
	[SerializeField]
	public int[] weaponCounts = null;

	// Crafts and counts are indexed based on the CraftableObject's ID
	[SerializeField]
	public Armor[] armors = null;
	[SerializeField]
	public int[] armorCounts = null;

    /*
     * The equipment array is structured like the following:
     *   [0]: Weapon 1
     *   [1]: Weapon 2
     *   [2]: Free slot (Melee is now deprecated)
     *   [3]: Throwable
     *   [4]: Item
     */
    public Equipment[] equipment = null;
	public Armor equippedArmor = null;
	private List<ResourcePersistent> resources = null;
	private HashSet<Resource> resourceSet = null;
    public bool isReloading = false;

    public int currentIndex;

    private GameObject currentObject;

    public delegate void EquipmentSwitched();
    public event EquipmentSwitched OnEquipmentSwitched;

    void Start()
    {
        currentIndex = -1;
        sceneManager = GetComponent<PlayerSceneManager>();
        inGameManager = GetComponent<InGameDataManager>();

        equipment = new Equipment[5];
        //equipment[0] = weapons[(int)WeaponType.AR];
        equipment[0] = weapons[(int)WeaponType.Pistol];
		//weaponCounts[(int)WeaponType.AR]++;
		weaponCounts[(int)WeaponType.Pistol]++;

        resources = new List<ResourcePersistent>();
		resourceSet = new HashSet<Resource>();
    }

    void Update()
    {
        if (!photonView.IsMine)
            return;
        if (sceneManager.isInHomeBase)
            return;
    }

    #region Setup

    public void SetupInScene()
    {
        SetupEquipment();
    }

    private void SetupEquipment()
    {
        for (int i = 0; i < equipment.Length; i++)
        {
            var equip = equipment[i];

            if (equip != null && !(equip is Item))
            {
                Transform parent;
                if (i == 0 || i == 1) parent = gunParent;
                else if (i == 2) parent = meleeParent;
                else if (i == 3) parent = grenadeParent;
                else parent = medShotParent;

                if (equip.id == (int) WeaponType.Frag)
                {
                    if (!photonView.IsMine) continue;
                    GameObject obj = PhotonNetwork.Instantiate("PhotonPrefabs/Grenade", grenadeParent.position, Quaternion.identity);
                    photonView.RPC("InstantiateGrenade", RpcTarget.All, obj.GetPhotonView().ViewID);
                } 
                else if (equip.id == (int) WeaponType.Sticky)
                {
                    if (!photonView.IsMine) continue;
                    GameObject obj = PhotonNetwork.Instantiate("PhotonPrefabs/Sticky", grenadeParent.position, Quaternion.identity);
                    photonView.RPC("InstantiateGrenade", RpcTarget.All, obj.GetPhotonView().ViewID);
                }
                else
                {
                    GameObject newObject = Instantiate(equip.prefab, parent.position, parent.rotation, parent);
                    newObject.transform.localPosition = Vector3.zero;
                    newObject.transform.localEulerAngles = Vector3.zero;
                    newObject.SetActive(false);
                }
                
            }
        }

        if (photonView.IsMine)
            photonView.RPC("EquipWeapon", RpcTarget.All, 0);
    }

    #endregion Setup

    [PunRPC]
    public void InstantiateGrenade(int photonId)
    {
        GameObject grenade = PhotonView.Find(photonId).gameObject;
        grenade.transform.SetParent(grenadeParent);
        grenade.transform.localPosition = Vector3.zero;
        grenade.transform.localEulerAngles = Vector3.zero;
        grenade.SetActive(false);
    }


	[PunRPC]
    public void ClearEquipmentOnDeath()
    {
		if (equipment[0] != null) weaponCounts[equipment[0].id]--;
		if (equipment[1] != null) weaponCounts[equipment[1].id]--;
		if (equipment[2] != null) weaponCounts[equipment[2].id]--;  // Not really needed but sure why not.
		if (equipment[3] != null) weaponCounts[equipment[3].id]--;
		if (equippedArmor != null) armorCounts[equippedArmor.id]--;
		if (equipment[4] != null) itemCounts[equipment[4].id]--;

		equipment = new Equipment[5];
        equipment[0] = weapons[(int)WeaponType.Pistol];
		weaponCounts[(int)WeaponType.Pistol] = 1;
		equippedArmor = null;
    }
	public Equipment[] getEquipment()
	{
		return equipment;
	}

	public List<ResourcePersistent> getResources()
	{
		return resources;
	}

	public HashSet<Resource> getResourceSet()
	{
		return resourceSet;
	}

	public void AddResourceToStorage(Resource r, int count)
	{
		if (photonView.IsMine && resources.Count < 44) {
			if (!resourceSet.Contains(r)) {
				resources.Add(new ResourcePersistent(r, count));
				resourceSet.Add(r);
			} else {
				ResourcePersistent old = null;
				foreach (ResourcePersistent re in resources) {
					if (re.Resource == r) {
						old = re;
					}
				}
				int idx = resources.IndexOf(old);
				resources.RemoveAt(idx);
				resources.Insert(idx, new ResourcePersistent(r, old.Count + count));
			}
		}
	}

	public void RemoveResourceFromStorage(Resource r, int count)
	{
		if (photonView.IsMine) {
			ResourcePersistent rp = null;
			foreach (ResourcePersistent re in resources) {
				if (re.Resource == r) {
					re.Count -= count;
					if (re.Count <= 0) {
						rp = re;
					}
					break;
				}
			}
			if (rp != null) {
				resources.Remove(rp);
				resourceSet.Remove(rp.Resource);
			}
		}
	}

	[PunRPC]
	public void TransferToInGame(int[] equipEnums, int armorEnum) {
        for (int i = 0; i < 4; i++)
        {
            if (equipEnums[i] == -1)
            {
                equipment[i] = null;
                inGameManager.currentWeapons[i] = null;
            }
            else
            {
                equipment[i] = weapons[equipEnums[i]];
                inGameManager.currentWeapons[i] = weapons[equipEnums[i]];
            }
        }
        // Array.Copy(equipment, 0, inGameManager.currentWeapons, 0, 4);
        equipment[4] = equipEnums[4] == -1 ? null : items[equipEnums[4]];
        inGameManager.currentItem = equipEnums[4] == -1 ? null : items[equipEnums[4]];

        equippedArmor = armorEnum == -1 ? null : armors[armorEnum];
        inGameManager.currentArmor = armorEnum == -1 ? null : armors[armorEnum];
    }
}
