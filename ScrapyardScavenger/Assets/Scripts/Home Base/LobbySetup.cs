using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class LobbySetup : MonoBehaviourPun
{
    void Start()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameController");
        foreach (var obj in objs)
        {
            obj.GetComponent<PlayerSceneManager>().isInHomeBase = true;
        }

        
        PhotonNetwork.Instantiate("PhotonPrefabs/PlayerController", Vector3.zero, Quaternion.identity);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
