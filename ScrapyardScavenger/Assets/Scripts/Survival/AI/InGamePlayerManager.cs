using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class InGamePlayerManager : MonoBehaviour
{
    public List<GameObject> players;

    public GameObject deathUI;
    public GameObject UI;
    public GameObject winCam;
    public GameObject deathCam;
    public int homeBaseSceneIndex;

    private Boolean escaped = false;

    private void Awake()
    {
        players = new List<GameObject>();
    }

    public GameObject GetOtherPlayer()
    {
        foreach (var player in players)
        {
            if (!player.GetPhotonView().IsMine)
            {
                return player;
            }
        }

        throw new Exception("You asked for another player when another player does not exist!");
    }

    public void Register(GameObject adding)
    {
        players.Add(adding);
        adding.GetComponent<Death>().OnPlayerDeath += PlayerDied;
    }


    public void PlayerDied(GameObject deadPlayer)
    {
        players.Remove(deadPlayer);

        Debug.Log("Died");
        GameControllerSingleton.instance.aliveCount--;
        if (GameControllerSingleton.instance.aliveCount <= 0)
        {
            StartCoroutine(InitiateLoss());
        }
    }

    public IEnumerator InitiateLoss()
    {
        Instantiate(deathUI);

        NotificationSystem.Instance.Notify(new Notification("Your team died!", NotificationType.Bad));

        yield return new WaitForSeconds(2);

        PhotonNetwork.LoadLevel(homeBaseSceneIndex);
    }

    public void PlayersEscaped()
    {
        if (escaped)
        {
            Debug.Log("ESCAPED ALREADY");
            return;
        }
        Debug.Log("NOT ESCAPED YET");
        escaped = true;

        Debug.Log("CAM ACTIVE");
        UI.SetActive(false);

        winCam.SetActive(true);
        Animator winAnimator = winCam.GetComponent<Animator>();
        winAnimator.speed = 1 / 8.0f;
        winAnimator.Play("Move");

        Instantiate(deathUI);

        List<GameObject> playerControllers = new List<GameObject>();
        foreach (GameObject player in players)
        {
            if (player == null) continue;

            GameObject pController = player.GetComponent<PlayerControllerLoader>().playerController;
            playerControllers.Add(pController);

            Destroy(player);
        }

        

        NotificationSystem.Instance.Notify(new Notification("You have Escaped!", NotificationType.Good));

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(EscapeRoutine(playerControllers));
        }

    }

    public IEnumerator EscapeRoutine(List<GameObject> playerControllers)
    {
        yield return new WaitForSeconds(7);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        foreach (GameObject playerController in playerControllers)
        {
            playerController.GetPhotonView().RPC("MasterClientGoToHomeBase", RpcTarget.All);
        }
    }
}
