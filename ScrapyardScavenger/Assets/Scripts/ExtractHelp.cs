using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class ExtractHelp : MonoBehaviourPunCallbacks
{

    public int homebaseIndex;
    public float leaveRadius;

    private GameObject evacCircle;
    private bool otherPlayerWantsToLeave;
    private Coroutine leaveCoroutine;
    private bool isLeaving;


    // Start is called before the first frame update
    void Start()
    {
        otherPlayerWantsToLeave = false;
        isLeaving = false;
    }

    public void IAmReadyToLeave()
    {
        isLeaving = true;
        SpawnCircle();
    }

    public void FirstPlayerReadyToLeave()
    {
        otherPlayerWantsToLeave = true;
        SpawnCircle();
    }

    public void SecondPlayerReadyToLeave()
    {
        isLeaving = true;

        // start the countdown
        leaveCoroutine = StartCoroutine(LeaveGame());
    }

    public void CancelLeave()
    {
        Destroy(evacCircle);
        if (leaveCoroutine != null)
            StopCoroutine(leaveCoroutine);
        isLeaving = false;

        otherPlayerWantsToLeave = false; // ?
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
        Debug.Log("Leaving Game");
        float time = 0;
        float totalWaitTime = 4.0f;
        GameObject evacuateCanvas = GameObject.Find("Exit Canvas");
        while (time < totalWaitTime)
        {
            evacuateCanvas.GetComponentInChildren<Text>().text = "Leaving in... " + (totalWaitTime - time);
            Debug.Log($"{totalWaitTime - time}");
            time++;
            yield return new WaitForSeconds(1);
        }
        isLeaving = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(homebaseIndex);
        }
        
    }

    public void SpawnCircle()
    {
        GameObject truck = GameObject.Find("ExtractionTruck");
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
