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

    public int homebaseIndex;
    public float leaveRadius;

    private GameObject evacCircle;
    private bool otherPlayerWantsToLeave;
    private Coroutine leaveCoroutine;
    private bool isLeaving;

    private GameObject evacuateCanvas;
    private GameObject truck;

    private bool isLookingAtTruck;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerControllerLoader>().playerController;

        otherPlayerWantsToLeave = false;
        isLeaving = false;
        isLookingAtTruck = false;

        evacuateCanvas = GameObject.Find("Exit Canvas");
        truck = GameObject.Find("ExtractionTruck");

        GetComponent<Death>().OnPlayerDeath += OnDeath;
    }

    #region Regular Update

    void Update()
    {
        if (!photonView.IsMine) return;

        ExtractionCheck();
    }

    private void ExtractionCheck()
    {
        if (LookingAtTruck() && !IsOtherPlayerLeaving() && !IsLeaving())
        {
            // show button pop up
            evacuateCanvas.GetComponentInChildren<Text>().text = "Press B to escape!";
            isLookingAtTruck = true;
        }
        // if both players are not leaving
        else if (!IsLeaving() && !IsOtherPlayerLeaving())
        {
            // remove the button pop up
            evacuateCanvas.GetComponentInChildren<Text>().text = ""; // SetActive(false);
            isLookingAtTruck = false;
        }

        if (IsOtherPlayerLeaving() && !IsLeaving())
        {
            // the other player wants to leave and you are not ready yet, so
            // check to see if you are within the circle or not
            // by calculating the distance between the player and the truck
            float dist = Vector3.Distance(truck.transform.position, transform.position);
            if (dist <= (leaveRadius + 0.5f))
            {
                // inside the circle so notify the other player they can leave
                GetComponent<PlayerManager>().inGamePlayerManager.GetOtherPlayer().GetPhotonView().RPC("SecondPlayerReadyToLeave", RpcTarget.All);
                photonView.RPC("SecondPlayerReadyToLeave", RpcTarget.All);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.B) && isLookingAtTruck && !IsLeaving())
            {
                // if there are 2 players alive, signal the other player that you are leaving
                if (GameControllerSingleton.instance.aliveCount == 2)
                {
                    GetComponent<PlayerManager>().inGamePlayerManager.GetOtherPlayer().GetPhotonView().RPC("FirstPlayerReadyToLeave", RpcTarget.All);
                    IAmReadyToLeave();

                    // reset the text
                    evacuateCanvas.GetComponentInChildren<Text>().text = "Waiting for other player";

                }

                // if only 1 player, just start the countdown
                else
                {
                    SoloLeave();
                }
            }
        }


        // now check to see if the player is trying to escape from the circle
        if (IsLeaving())
        {
            // check if the player has left the escape circle
            // by calculating the distance between the player and the truck
            float dist = Vector3.Distance(truck.transform.position, transform.position);
            if (dist > (leaveRadius + 0.5f))
            {
                // outside of the circle?
                // cancel the leaving
                photonView.RPC("CancelLeave", RpcTarget.All);
                // if there are 2 players, signal the other
                if (GameControllerSingleton.instance.aliveCount == 2)
                {
                    GetComponent<PlayerManager>().inGamePlayerManager.GetOtherPlayer().GetPhotonView().RPC("CancelLeave", RpcTarget.All);
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
        if (Physics.Raycast(eyeCam.position, eyeCam.forward, out hit, 2.5f, layerMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #region Events

    public void OnDeath(GameObject deadPlayer)
    {
        photonView.RPC("CancelLeave", RpcTarget.All);
        GetComponent<PlayerManager>().inGamePlayerManager.GetOtherPlayer().GetPhotonView().RPC("CancelLeave", RpcTarget.All);
    }

    #endregion

    public void IAmReadyToLeave()
    {
        isLeaving = true;
        SpawnCircle();
    }

    [PunRPC]
    public void FirstPlayerReadyToLeave()
    {
        if (photonView.IsMine)
        {
            otherPlayerWantsToLeave = true;
            SpawnCircle();
        }
    }

    [PunRPC]
    public void SecondPlayerReadyToLeave()
    {
        if (photonView.IsMine)
        {
            isLeaving = true;

            // start the countdown
            leaveCoroutine = StartCoroutine(LeaveGame());
        }
    }

    [PunRPC]
    public void CancelLeave()
    {
        PhotonNetwork.Destroy(evacCircle);
        if (photonView.IsMine)
        {
            if (leaveCoroutine != null)
                StopCoroutine(leaveCoroutine);
            isLeaving = false;
        }

        otherPlayerWantsToLeave = false;
    }

    public bool IsOtherPlayerLeaving()
    {
        return otherPlayerWantsToLeave;
    }

    public bool IsLeaving()
    {
        return isLeaving;
    }

    public void SoloLeave()
    {
        isLeaving = true;
        SpawnCircle();
        leaveCoroutine = StartCoroutine(LeaveGame());
    }

    public void SetLeaving(bool leave)
    {
        isLeaving = leave;
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
        isLeaving = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
       
        // Return to Home Base
        playerController.GetPhotonView().RPC("MasterClientGoToHomeBase", RpcTarget.All);
        
        if (photonView.IsMine)
            PhotonNetwork.Destroy(this.gameObject);
    }

    public void SpawnCircle()
    {
        GameObject truck = GameObject.Find("ExtractionTruck");

        // if (GameObject.FindGameObjectsWithTag("ExtractionCircle").Length == 0)
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
