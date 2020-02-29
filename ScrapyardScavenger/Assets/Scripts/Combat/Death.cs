using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Death : MonoBehaviourPun
{
    private bool isDead;
    public Camera deathCam;
    private GameObject UI;

    void Start()
    {
        isDead = false;
        UI = GameObject.Find("In-Game UI");
    }

    void Update()
    {
        
    }

    public void hasDied()
    {
        isDead = true;
        GetComponent<PlayerMotor>().normalCam = deathCam;
        GetComponent<PlayerMotor>().speed = 0;
        GetComponent<PlayerMotor>().speedModifier = 0;
        GetComponent<PlayerMotor>().sprintModifier = 0;
        GetComponent<PlayerMotor>().jumpForce = 0;
        UI.SetActive(false);
        PhotonNetwork.Destroy(this.gameObject);
    }
}
