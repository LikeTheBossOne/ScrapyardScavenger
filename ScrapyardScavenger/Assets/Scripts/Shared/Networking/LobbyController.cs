using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class LobbyController : MonoBehaviourPunCallbacks
{
    #region Survival Fields

    [SerializeField] private GameObject survivalPlayButton;
    [SerializeField] private GameObject survivalCancelButton;
    [SerializeField] private int survivalRoomSize;
    private TypedLobby survivalLobby;

    #endregion

    #region Faceoff Fields

    [SerializeField] private GameObject faceoffPlayButton;
    [SerializeField] private GameObject faceoffCancelButton;
    [SerializeField] private int faceoffRoomSize;
    private TypedLobby faceoffLobby;

    #endregion

    void Start()
    {
        survivalLobby = new TypedLobby("Survival", LobbyType.Default);
        faceoffLobby = new TypedLobby("Faceoff", LobbyType.Default);
    }


    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        survivalPlayButton.SetActive(true);
        faceoffPlayButton.SetActive(true);
    }

    public void PlaySurvival()
    {
        survivalPlayButton.SetActive(false);
        survivalCancelButton.SetActive(true);
        faceoffPlayButton.SetActive(false);
        faceoffCancelButton.SetActive(false);
        PhotonNetwork.JoinRandomRoom(null, 0, MatchmakingMode.FillRoom, survivalLobby, null, null);
    }

    public void PlayFaceoff()
    {
        survivalPlayButton.SetActive(false);
        survivalCancelButton.SetActive(false);
        faceoffPlayButton.SetActive(false);
        faceoffCancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom(null, 0, MatchmakingMode.FillRoom, faceoffLobby, null, null);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a room");

        // In survival mode
        if (survivalCancelButton.activeSelf)
        {
            CreateRoom(survivalLobby);
        }
        else if (faceoffCancelButton.activeSelf)
        {
            CreateRoom(faceoffLobby);
        }
        
    }

    void CreateRoom(TypedLobby type)
    {
        Debug.Log("Creating room");
        int randomRoomNumber = Random.Range(0, 10000);

        RoomOptions roomOps;
        roomOps = type.Name == "Survival" ?
            new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)survivalRoomSize } :
            new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)faceoffRoomSize };
        
        PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOps, type);
        Debug.Log(randomRoomNumber);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room... trying again");
        if (survivalCancelButton.activeSelf)
        {
            CreateRoom(survivalLobby);
        }
        else if (faceoffCancelButton.activeSelf)
        {
            CreateRoom(faceoffLobby);
        }
    }

    public void CancelSurvival()
    {
        survivalCancelButton.SetActive(false);
        survivalPlayButton.SetActive(true);
        faceoffPlayButton.SetActive(true);
        faceoffCancelButton.SetActive(false);
        PhotonNetwork.LeaveRoom();
    }

    public void CancelFaceoff()
    {
        survivalCancelButton.SetActive(false);
        survivalPlayButton.SetActive(true);
        faceoffPlayButton.SetActive(true);
        faceoffCancelButton.SetActive(false);
        PhotonNetwork.LeaveRoom();
    }
}
