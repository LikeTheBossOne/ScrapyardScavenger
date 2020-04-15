using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class HomeBaseNetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject playerController;

    public int multiplayerIndex;

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

        if (readyCount == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            GameObject[] buttons = GameObject.FindGameObjectsWithTag("HomeScreenButton");
            foreach (GameObject button in buttons)
            {
                button.GetComponent<Button>().interactable = false;
            }

			if (playerController == null) {
				foreach (GameObject obj in GameObject.FindGameObjectsWithTag("GameController"))
				{
					if (obj.GetPhotonView().IsMine)
					{
						playerController = obj;
						break;
					}
				}
			}
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
                StartCoroutine(StartGame());
            }

            BaseDataManager bData = playerController.GetComponent<BaseDataManager>();
            int[] equipmentEnums = new int[5];
            for (int i = 0; i < bData.equipment.Length; i++)
            {
                if (bData.equipment[i] == null)
                {
                    equipmentEnums[i] = -1;
                }
                else
                {
                    equipmentEnums[i] = bData.equipment[i].id;
                }
            }

            int armorEnum;
            if (bData.equippedArmor == null)
            {
                armorEnum = -1;
            }
            else
            {
                armorEnum = bData.equippedArmor.id;
            }
            playerController.GetPhotonView().RPC("TransferToInGame", RpcTarget.All, equipmentEnums, armorEnum);
            
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

    public void SetReadyText()
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

		playerController.GetComponent<InGameDataManager>().refreshInv = false;

        PhotonNetwork.LoadLevel(multiplayerIndex);
    }

    #endregion
}
