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

    void Update()
    {
        
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
