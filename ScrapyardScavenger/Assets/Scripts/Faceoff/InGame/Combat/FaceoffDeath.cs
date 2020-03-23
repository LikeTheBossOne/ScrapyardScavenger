using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class FaceoffDeath : MonoBehaviourPun
{
    public int faceoffReadySceneIndex;
    public GameObject deathUI;

    private GameObject UI;

    public delegate void PlayerDeath(GameObject player, TeamGroup team);
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
    public void PlayerDied(int actor)
    {
        TeamGroup team = GetComponent<Team>().team;

        // Notification
        if (actor != -1)
        {
            NotificationType notifType = NotificationType.Good;

            FaceoffPlayerManager pManager = FindObjectOfType<FaceoffPlayerManager>();
            // If my client is the dead player
            if (photonView.IsMine)
                notifType = NotificationType.Bad;
            // Or if the dead player has a teammate who is my client
            else
            {
                IEnumerable<GameObject> teamMates = pManager.GetTeammates(gameObject, team);
                foreach (var mate in teamMates)
                {
                    if (mate.GetPhotonView().IsMine)
                    {
                        notifType = NotificationType.Bad;
                        break;
                    }
                }
            }

            NotificationSystem.Instance.Notify(new Notification($"Player {actor} killed Player {photonView.ControllerActorNr}", notifType));
        }

        if (photonView.IsMine)
            Instantiate(deathUI);


        // Hit all death events
        try
        {
            OnPlayerDeath?.Invoke(gameObject, team);
        }
        catch (Exception)
        {
            Debug.Log("Exception");
        }



        if (photonView.IsMine)
        {
            // GetComponent<PlayerMotor>().normalCam = deathCam;
            GetComponent<PlayerMotor>().speed = 0;
            GetComponent<PlayerMotor>().speedModifier = 0;
            GetComponent<PlayerMotor>().sprintModifier = 0;
            GetComponent<PlayerMotor>().jumpForce = 0;

            UI.SetActive(false);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (photonView.IsMine)
            PhotonNetwork.Destroy(this.gameObject);
    }
}
