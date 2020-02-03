using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class EquipmentManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public Transform gunParent;
    public Transform meleeParent;
    public Transform grenadeParent;
    public Transform medShotParent;

    [SerializeField]
    private Equipment[] equipment;

    private int currentIndex;
    private GameObject currentObject;

    public delegate void OnEquipmentSwitchedDelegate();
    public static OnEquipmentSwitchedDelegate EquipmentSwitched;

    #region Singleton

    public static EquipmentManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    #endregion

    void Start()
    {
        currentIndex = -1;
        if (!photonView.IsMine)
        {
            byte evCode = 1; // Custom Event 1: Used as "MoveUnitsToTargetPosition" event
            object[] content = new object[] { }; // Array contains the target position and the IDs of the selected units
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others }; // You would have to set the Receivers to All in order to receive this event on the local client as well
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
        }
    }

    void Update()
    {
        if (!photonView.IsMine)
            return;


        if (Input.GetKeyDown(KeyCode.Alpha1) && currentIndex != 0)
            photonView.RPC("Equip", RpcTarget.All, 0);
        if (Input.GetKeyDown(KeyCode.Alpha2) && currentIndex != 1)
            photonView.RPC("Equip", RpcTarget.All, 1);
        if (Input.GetKeyDown(KeyCode.Alpha3) && currentIndex != 2)
            photonView.RPC("Equip", RpcTarget.All, 2);
        if (Input.GetKeyDown(KeyCode.Alpha4) && currentIndex != 3)
            photonView.RPC("Equip", RpcTarget.All, 3);
        if (Input.GetKeyDown(KeyCode.Alpha5) && currentIndex != 4)
            photonView.RPC("Equip", RpcTarget.All, 4);
    }

    [PunRPC]
    void Equip(int index)
    {
        if (currentObject != null)
            Destroy(currentObject);


        Transform parent;
        if (index == 0 || index == 1) parent = gunParent;
        else if (index == 2) parent = meleeParent;
        else if (index == 3) parent = grenadeParent;
        else if (index == 4) parent = medShotParent;
        else return;

        

        GameObject newObject = Instantiate(equipment[index].prefab, parent.position, parent.rotation, parent);
        newObject.transform.localPosition = Vector3.zero;
        newObject.transform.localEulerAngles = Vector3.zero;

        currentIndex = index;
        currentObject = newObject;
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == 1)
        {
            if (photonView.IsMine)
            {
                byte evCode = 2; // Sending equipmentData
                object[] content = new object[] { currentIndex, photonView.ViewID }; // Array contains the target position and the IDs of the selected units
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others }; // You would have to set the Receivers to All in order to receive this event on the local client as well
                SendOptions sendOptions = new SendOptions { Reliability = true };
                PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
            }
        }

        else if (eventCode == 2)
        {
            object[] data = (object[])photonEvent.CustomData;
            if (photonView.ViewID == (int)data[1])
                Equip((int)data[0]);
        }
    }
}
