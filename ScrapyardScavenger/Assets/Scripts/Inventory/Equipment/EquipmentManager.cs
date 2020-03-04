using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class EquipmentManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public PlayerSceneManager sceneManager;
    private InventoryManager inventoryManager;

    public Transform gunParent;
    public Transform meleeParent;
    public Transform grenadeParent;
    public Transform medShotParent;

    private Equipment[] equipment = null;
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
        inventoryManager = GetComponent<InventoryManager>();

        equipment = new Equipment[5];
        equipment[0] = inventoryManager.weapons[(int)WeaponType.AR];
        equipment[1] = inventoryManager.weapons[(int)WeaponType.Pistol];

		resources = new List<ResourcePersistent>();
		resourceSet = new HashSet<Resource>();
    }

    void Update()
    {
        if (!photonView.IsMine)
            return;
        if (sceneManager.isInHomeBase)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1)
            && currentIndex != 0
            && equipment[0] != null)
            photonView.RPC("Equip", RpcTarget.All, 0);
        if (Input.GetKeyDown(KeyCode.Alpha2)
            && currentIndex != 1
            && equipment[1] != null)
            photonView.RPC("Equip", RpcTarget.All, 1);
        if (Input.GetKeyDown(KeyCode.Alpha3)
            && currentIndex != 2
            && equipment[2] != null)
            photonView.RPC("Equip", RpcTarget.All, 2);
        if (Input.GetKeyDown(KeyCode.Alpha4)
            && currentIndex != 3
            && equipment[3] != null)
            photonView.RPC("Equip", RpcTarget.All, 3);
        if (Input.GetKeyDown(KeyCode.Alpha5)
            && currentIndex != 4
            && equipment[4] != null)
            photonView.RPC("Equip", RpcTarget.All, 4);
    }

    #region Setup

    public void SetupInScene()
    {
        PlayerJoin();
        SetupEquipment();
    }

    private void PlayerJoin()
    {
        if (!photonView.IsMine)
        {
            object[] content = new object[] { };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent((byte)NetworkCodes.PlayerJoined, content, raiseEventOptions, sendOptions);
        }
    }

    private void SetupEquipment()
    {
        for (int i = 0; i < equipment.Length; i++)
        {
            var equip = equipment[i];

            if (equip != null)
            {
                Transform parent;
                if (i == 0 || i == 1) parent = gunParent;
                else if (i == 2) parent = meleeParent;
                else if (i == 3) parent = grenadeParent;
                else parent = medShotParent;

                GameObject newObject = Instantiate(equip.prefab, parent.position, parent.rotation, parent);
                newObject.transform.localPosition = Vector3.zero;
                newObject.transform.localEulerAngles = Vector3.zero;
                newObject.SetActive(false);
            }
        }

        if (photonView.IsMine)
            photonView.RPC("Equip", RpcTarget.All, 0);
    }

    #endregion Setup

    [PunRPC]
    void Equip(int index)
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
        currentIndex = index;

        OnEquipmentSwitched?.Invoke();
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == (byte) NetworkCodes.PlayerJoined)
        {
            if (photonView.IsMine)
            {
                object[] content = new object[] { currentIndex, photonView.ViewID };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.Others};
                SendOptions sendOptions = new SendOptions { Reliability = true };
                PhotonNetwork.RaiseEvent((byte) NetworkCodes.PlayerJoinedResponse, content, raiseEventOptions, sendOptions);
            }
        }

        else if (eventCode == (byte) NetworkCodes.PlayerJoinedResponse)
        {
            object[] data = (object[])photonEvent.CustomData;
            if (photonView.ViewID == (int)data[1])
                Equip((int)data[0]);
        }
    }

    public Equipment getCurrentEquipment()
    {
        if (currentIndex == -1) return null;
        return equipment[currentIndex];
    }

    public void Clear()
    {
        equipment = new Equipment[5];
        equipment[0] = inventoryManager.weapons[(int)WeaponType.AR];
        equipment[1] = inventoryManager.weapons[(int)WeaponType.Pistol];
    }
	public Equipment[] getEquipment()
	{
		return equipment;
	}

	public List<ResourcePersistent> getResources()
	{
		return resources;
	}

	public void AddResource(Resource r, int count)
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
				resources.Insert(idx, new ResourcePersistent(r, count));
			}
		}
	}
}
