using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerControllerLoader : MonoBehaviourPun
{
    public GameObject playerController;
    public EquipmentManager equipmentManager;

    public Transform gunParent;
    public Transform meleeParent;
    public Transform grenadeParent;
    public Transform medParent;

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameController");
        foreach (var obj in objs)
        {
            if (obj.GetPhotonView().Owner.UserId == photonView.Owner.UserId)
            {
                playerController = obj;
                equipmentManager = obj.GetComponent<EquipmentManager>();
            }
        }

        playerController.GetComponent<PlayerSceneManager>().isInHomeBase = false;
    }

    void Start()
    {
        equipmentManager.gunParent = gunParent;
        equipmentManager.SetupInScene();
    }

    void Update()
    {
        
    }
}
