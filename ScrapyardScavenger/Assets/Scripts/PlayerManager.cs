using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public InGamePlayerManager inGamePlayerManager;

    void Start()
    {
        inGamePlayerManager = FindObjectOfType<InGamePlayerManager>();

        inGamePlayerManager.Register(gameObject);
    }
}
