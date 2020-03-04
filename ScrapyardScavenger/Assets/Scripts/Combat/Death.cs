using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Death : MonoBehaviourPun
{
    public Camera deathCam;
    public int homeBaseSceneIndex;
    
    private GameObject UI;

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
        if (photonView.IsMine)
        {
            GetComponent<PlayerMotor>().normalCam = deathCam;
            GetComponent<PlayerMotor>().speed = 0;
            GetComponent<PlayerMotor>().speedModifier = 0;
            GetComponent<PlayerMotor>().sprintModifier = 0;
            GetComponent<PlayerMotor>().jumpForce = 0;
            UI.SetActive(false);

            PlayerControllerLoader pControllerLoader = GetComponent<PlayerControllerLoader>();
            pControllerLoader.equipmentManager.Clear();
            pControllerLoader.inventoryManager.Clear();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }


        GameControllerSingleton.instance.aliveCount--;
        if (GameControllerSingleton.instance.aliveCount <= 0)
        {
            PhotonNetwork.LoadLevel(homeBaseSceneIndex);
        }

        if (photonView.IsMine)
            PhotonNetwork.Destroy(this.gameObject);
    }
}
