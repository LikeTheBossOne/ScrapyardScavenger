using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class LobbySetup : MonoBehaviourPun
{
    void Start()
    {
        PhotonNetwork.Instantiate("PhotonPrefabs/PlayerController", Vector3.zero, Quaternion.identity);
    }

}
