using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Death : MonoBehaviourPun
{
    private GameObject UI;

    public delegate void PlayerDeath(GameObject player);
    public event PlayerDeath OnPlayerDeath;

    void Start()
    {
        UI = GameObject.Find("In-Game UI");
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            photonView.RPC("PlayerDied", RpcTarget.All);
        }
    }

    [PunRPC]
    public void PlayerDied()
    {
		// Hit all death events
        try
        {
            OnPlayerDeath?.Invoke(gameObject);
        }
        catch (Exception)
        {
            Debug.Log("Exception");
        }
        


        if (photonView.IsMine)
        {
            GameObject deathCam = GameObject.Find("PlayerList").GetComponent<InGamePlayerManager>().deathCam;
            deathCam.SetActive(true);
            GetComponent<PlayerMotor>().normalCam = deathCam.GetComponent<Camera>();
            GetComponent<PlayerMotor>().speed = 0;
            GetComponent<PlayerMotor>().speedModifier = 0;
            GetComponent<PlayerMotor>().sprintModifier = 0;
            GetComponent<PlayerMotor>().jumpForce = 0;

            PlayerControllerLoader pControllerLoader = GetComponent<PlayerControllerLoader>();
			pControllerLoader.playerController.GetPhotonView().RPC("ClearEquipmentOnDeath", RpcTarget.All);
            pControllerLoader.inGameDataManager.ClearOnDeath();
            GetComponent<PlayerControllerLoader>().skillManager.ClearTempXP();

			UI.SetActive(false);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (photonView.IsMine)
            PhotonNetwork.Destroy(this.gameObject);
    }
}
