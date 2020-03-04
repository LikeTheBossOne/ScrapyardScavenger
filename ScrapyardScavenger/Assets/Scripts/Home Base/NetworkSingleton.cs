using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NetworkSingleton : MonoBehaviourPun
{
    public static int singletonCount;

    void Awake()
    {
        if (singletonCount >= PhotonNetwork.CurrentRoom.PlayerCount)
            Destroy(this.gameObject);
        else
            singletonCount++;
    }
    
}
