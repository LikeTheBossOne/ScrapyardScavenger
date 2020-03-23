using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class FaceoffLobbyState : MonoBehaviourPunCallbacks
{
    public int faceoffSceneIndex;
    public Text readyAmountText;
    public Text teamText;
    public GameObject readyButton;

    private int readyCount = 0;


    void Start()
    {
        SetReadyText();
        int pNum = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        teamText.text = $"Team {pNum % 2 + 1}";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    #region RPCs

    [PunRPC]
    void PlayerReadied()
    {
        readyCount++;
        SetReadyText();

        if (readyCount == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            GameObject[] buttons = GameObject.FindGameObjectsWithTag("HomeScreenButton");
            foreach (GameObject button in buttons)
            {
                button.GetComponent<Button>().interactable = false;
            }

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
                StartCoroutine(StartGame());
            }
        }
    }

    [PunRPC]
    public void PlayerNotReadied()
    {
        readyCount--;
        SetReadyText();
    }

    #endregion

    #region Pun Callbacks

    public override void OnPlayerEnteredRoom(Player player)
    {
        SetReadyText();
    }

    public override void OnPlayerLeftRoom(Player player)
    {
        if (readyButton.activeSelf)
        {
            if (readyCount > 0)
                photonView.RPC("PlayerNotReadied", RpcTarget.MasterClient);
        }
        else
        {
            if (readyCount > 1)
                photonView.RPC("PlayerNotReadied", RpcTarget.MasterClient);
        }
        SetReadyText();
    }

    #endregion

    #region Private Methods

    private void SetReadyText()
    {
        readyAmountText.text = $"{readyCount}/{PhotonNetwork.CurrentRoom.PlayerCount} Players Ready";
    }

    private IEnumerator StartGame()
    {
        Debug.Log("Starting Game");
        float time = 0;
        float totalWaitTime = 3;
        while (time < totalWaitTime)
        {
            Debug.Log($"{totalWaitTime - time}");
            time++;
            yield return new WaitForSeconds(1);
        }

        PhotonNetwork.LoadLevel(faceoffSceneIndex);
    }

    #endregion
}
