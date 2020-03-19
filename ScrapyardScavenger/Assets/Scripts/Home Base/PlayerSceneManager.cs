using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerSceneManager : MonoBehaviourPun
{
    public bool isInHomeBase;

    void Start()
    {
        isInHomeBase = true;
    }


    [PunRPC]
    public void MasterClientGoToHomeBase()
    {
        isInHomeBase = true;
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }
}
