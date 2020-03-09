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

//    void FixedUpdate()
//    {
//        // if there are 2 players, then get the other player once
//        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && otherPlayer == null)
//        {
//            // try and get the other player
//            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
//            if (players.Length != 0)
//            {
//                foreach(GameObject thisPlayer in players)
//                {
//                    if (thisPlayer.name == "Body") continue;
//                    Debug.Log("Player name: " + thisPlayer.name);
//                    if (!thisPlayer.GetPhotonView().IsMine)
//                    {
//                        Debug.Log("This is the other player");
//                        otherPlayer = thisPlayer;
//                        return;
//                    }
//                }
//            }
//        }
//    }
}
