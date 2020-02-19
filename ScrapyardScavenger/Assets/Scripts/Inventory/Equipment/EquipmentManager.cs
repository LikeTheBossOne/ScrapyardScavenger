﻿using System;
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
    private Equipment[] equipment = null;

    private int currentIndex;

    private GameObject currentObject;

    public delegate void OnEquipmentSwitchedDelegate();
    public static OnEquipmentSwitchedDelegate EquipmentSwitched;

    void Start()
    {
        currentIndex = -1;
        if (!photonView.IsMine)
        {
            object[] content = new object[] { };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent((byte) NetworkCodes.PlayerJoined, content, raiseEventOptions, sendOptions);
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
}
