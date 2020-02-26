using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class HomeBaseNetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject readyButton;
    public GameObject notReadyButton;

    public Text readyAmountText;

    public int readyCount = 0;

    void Start()
    {
        SetReadyText();
    }

    public void ReadyPressed()
    {
        Debug.Log("Ready");
        readyButton.SetActive(false);
        notReadyButton.SetActive(true);
        photonView.RPC("PlayerReadied", RpcTarget.All);
    }

    public void NotReadyPressed()
    {
        Debug.Log("Not Ready");
        readyButton.SetActive(true);
        notReadyButton.SetActive(false);
        photonView.RPC("PlayerNotReadied", RpcTarget.All);
    }


    #region RPCs

    [PunRPC]
    public void PlayerReadied()
    {
        readyCount++;
        SetReadyText();
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

    public void SetReadyText()
    {
        readyAmountText.text = $"{readyCount}/{PhotonNetwork.CurrentRoom.PlayerCount} Players Ready";
    }

    #endregion
}
