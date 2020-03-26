using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class RoomController : MonoBehaviourPunCallbacks
{
    [SerializeField] private int homeBaseSceneIndex;
    [SerializeField] private int faceoffReadySceneIndex;


    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        if (PhotonNetwork.CurrentRoom.MaxPlayers == 2)
        {
            StartSurvivalGame();
        }
        else
        {
            StartFaceoffGame();
        }
    }

    public void StartSurvivalGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Starting Survival Game");
            PhotonNetwork.LoadLevel(homeBaseSceneIndex);
        }
    }
    public void StartFaceoffGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Starting Faceoff Game");
            PhotonNetwork.LoadLevel(faceoffReadySceneIndex);
        }
    }
}
