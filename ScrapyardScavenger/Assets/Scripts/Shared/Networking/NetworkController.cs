﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonNetwork = Photon.Pun.PhotonNetwork;

public class NetworkController : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log($"Connected to the {PhotonNetwork.CloudRegion} server");
    }
}
