using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Extraction : MonoBehaviourPunCallbacks
{
    public GameObject evacCirclePrefab;
    private GameObject playerController;
	private GameObject otherPlayerController;

    public int homebaseIndex;
    public float leaveRadius;

    private GameObject evacCircle;
    private Coroutine leaveCoroutine;
    private bool leaving;
    private bool isLeader;

    private GameObject evacuateCanvas;
    private GameObject truck;

    private InGamePlayerManager playerManager;

    private bool isLookingAtTruck;

    // Start is called before the first frame update
    void Start()
    {
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("GameController"))
		{
			if (obj.GetPhotonView().IsMine)
			{
				playerController = obj;
			} else {
				otherPlayerController = obj;
			}
		}
		//playerController = GetComponent<PlayerControllerLoader>().playerController;

        leaving = false;
        isLookingAtTruck = false;
        isLeader = false;

        evacuateCanvas = GameObject.Find("Exit Canvas");
        truck = GameObject.Find("ExtractionTruck");

        GetComponent<Death>().OnPlayerDeath += OnDeath;

        playerManager = GameObject.Find("PlayerList").GetComponent<InGamePlayerManager>();
    }

    #region Regular Update

    void Update()
    {
        if (!photonView.IsMine) return;

        ExtractionCheck();
    }

    private void ExtractionCheck()
    {
        // check to see if player is looking at truck
        isLookingAtTruck = LookingAtTruck();


        // if neither player is leaving
        if (!IsLeaving() && !IsOtherPlayerLeaving())
        {
            if (isLookingAtTruck)
            {
                // show button pop up
				if (Input.GetJoystickNames().Length > 0 && Input.GetJoystickNames()[0] != "") {
					evacuateCanvas.GetComponentInChildren<Text>().text = "Press X to escape!";
				} else {
                	evacuateCanvas.GetComponentInChildren<Text>().text = "Press C to escape!";
				}
            }
            else
            {
                // remove the button pop up
                evacuateCanvas.GetComponentInChildren<Text>().text = "";
            }
        }
        // if the other player wants to leave and you are not ready yet
        else if (!IsLeaving() && IsOtherPlayerLeaving())
        {
            // check to see if you are within the circle or not
            // by calculating the distance between the player and the truck
            float dist = Vector3.Distance(truck.transform.position, transform.position);
            if (dist <= (leaveRadius + 0.5f))
            {
                // inside the circle so notify the other player you are ready to leave
                photonView.RPC("ReadyToLeave", RpcTarget.All);
            }
        }

        // if the player pressed the button to leave
		if ((Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown("joystick button 0")) && isLookingAtTruck && !IsLeaving())
        {
            photonView.RPC("ReadyToLeave", RpcTarget.All);
        }

        // now check to see if the player is trying to escape from the circle
        if (IsLeaving())
        {
            // check if the player has left the escape circle
            // by calculating the distance between the player and the truck
            float dist = Vector3.Distance(truck.transform.position, transform.position);
            if (dist > (leaveRadius + 0.5f))
            {
                // outside of the circle
                if (isLeader)
                {
                    photonView.RPC("CancelLeave", RpcTarget.All);
                }
                else
                {
                    // call cancel leave on the other player's photon view
                    leaving = false;
                    playerManager.GetOtherPlayer().GetPhotonView().RPC("CancelLeave", RpcTarget.All);
                }

            }
        }
    }


    bool LookingAtTruck()
    {
        LayerMask layerMask = LayerMask.GetMask("Truck");

        // This would cast rays only against colliders in ground 12.
        Transform eyeCam = transform.Find("Cameras/Main Player Cam");
        RaycastHit hit = new RaycastHit();
        return Physics.Raycast(eyeCam.position, eyeCam.forward, out hit, 2.5f, layerMask);
    }

    #endregion

    #region Events

    public void OnDeath(GameObject deadPlayer)
    {
        // if this is the dead player, call the RPC on normal
        deadPlayer.GetPhotonView().RPC("CancelLeave", RpcTarget.All);
        if (GameControllerSingleton.instance.aliveCount == 2)
        {
            if (gameObject == deadPlayer)
            {
                // then just get your other player and call it on that photon view
                GetComponent<PlayerManager>().inGamePlayerManager.GetOtherPlayer().GetPhotonView().RPC("CancelLeave", RpcTarget.All);
            }
            else
            {
                // call it on yourself
                photonView.RPC("CancelLeave", RpcTarget.All);
            }
        }
    }

    #endregion

    [PunRPC]
    public void ReadyToLeave()
    {
        if (photonView.IsMine)
        {
            leaving = true;
            if (evacCircle == null) SpawnCircle();

            if (GameControllerSingleton.instance.aliveCount == 1)
            {
                isLeader = true;
                leaveCoroutine = StartCoroutine(LeaveGame());
            }
            // see if the other player is ready to leave
            else if (IsOtherPlayerLeaving())
            {
                // then (as you are the 2nd person), start the countdown
                // on the other player's photonView
                playerManager.GetOtherPlayer().GetPhotonView().RPC("BeginLeave", RpcTarget.All);
            }
            else // other player is not ready to leave, but you are
            {
                isLeader = true;
                evacuateCanvas.GetComponentInChildren<Text>().text = "Waiting for other player";
            }
        }
        else
        {
            // the other player is ready to leave
            playerManager.GetOtherPlayer().GetComponent<Extraction>().SetLeaving(true);
            if (evacCircle == null) SpawnCircle();
        }
    }

    [PunRPC]
    public void BeginLeave()
    {
        Debug.Log("LEAVING");
        // start the countdown
        leaveCoroutine = StartCoroutine(LeaveGame());

    }

    [PunRPC]
    public void CancelLeave()
    {
        if (evacCircle != null)
            PhotonNetwork.Destroy(evacCircle);
        if (leaveCoroutine != null)
            StopCoroutine(leaveCoroutine);
        leaving = false;
        isLeader = false;
        if (GameControllerSingleton.instance.aliveCount == 2)
            playerManager.GetOtherPlayer().GetComponent<Extraction>().SetLeaving(false);

        // reset the UI's text since the dead player's UI won't update after this
        evacuateCanvas.GetComponentInChildren<Text>().text = "";
    }

    [PunRPC]
    public void Leave()
    {
        playerController.GetComponent<SkillManager>().TransferXP();
        playerController.GetComponent<InGameDataManager>().TransferToStorage();

        evacuateCanvas.SetActive(false);
        playerManager.PlayersEscaped();
        
    }

    public bool IsOtherPlayerLeaving()
    {
        if (GameControllerSingleton.instance.aliveCount != 2) return false;
        return playerManager.GetOtherPlayer().GetComponent<Extraction>().IsLeaving();
    }

    public bool IsLeaving()
    {
        return leaving;
    }

    public void SetLeaving(bool leave)
    {
        leaving = leave;
    }

    private IEnumerator LeaveGame()
    {
        float time = 0;
        float totalWaitTime = 4.0f;
        GameObject evacuateCanvas = GameObject.Find("Exit Canvas");
        while (time < totalWaitTime)
        {
            evacuateCanvas.GetComponentInChildren<Text>().text = "Leaving in... " + (totalWaitTime - time);
            time++;
            yield return new WaitForSeconds(1);
        }
        leaving = false;

        PhotonNetwork.Destroy(evacCircle);

        if (playerManager.players.Count > 1)
        {
            playerManager.GetOtherPlayer().GetPhotonView().RPC("Leave", RpcTarget.All);
        }
        photonView.RPC("Leave", RpcTarget.All);
    }

    public void SpawnCircle()
    {
        GameObject truck = GameObject.Find("ExtractionTruck");

        if (GameObject.FindGameObjectsWithTag("ExtractionCircle").Length > 0) return;
        Debug.Log("Spawning a circle");
        evacCircle = PhotonNetwork.Instantiate(Path.Combine("Extraction", "ExtractionCircle"), truck.transform.position, Quaternion.identity);


        float radius = leaveRadius; //7.5f;
        int numSegments = 128;
        LineRenderer lineRenderer = evacCircle.GetComponent<LineRenderer>();
        if (!lineRenderer.enabled) lineRenderer.enabled = true;
        Color c1 = new Color(1.0f, 0f, 0f, 1);
        //lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.startColor = c1;
        lineRenderer.endColor = c1;
        lineRenderer.startWidth = 0.5f;
        lineRenderer.endWidth = 0.5f;
        lineRenderer.positionCount = numSegments + 1;
        lineRenderer.useWorldSpace = false;

        float deltaTheta = (float)(2.0 * Mathf.PI) / numSegments;
        float theta = 0f;

        for (int i = 0; i < numSegments + 1; i++)
        {
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);
            Vector3 pos = new Vector3(x, 0, z);
            lineRenderer.SetPosition(i, pos);
            theta += deltaTheta;
        }
    }
}
