using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerControllerLoader : MonoBehaviourPun
{
    public GameObject playerController;
    public BaseDataManager equipmentManager;
    public InGameDataManager inventoryManager;
    public PlayerSceneManager sceneManager;
    public SkillManager skillManager;

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
                equipmentManager = obj.GetComponent<BaseDataManager>();
                inventoryManager = obj.GetComponent<InGameDataManager>();
                sceneManager = obj.GetComponent<PlayerSceneManager>();
                skillManager = obj.GetComponent<SkillManager>();
            }
        }

        sceneManager.isInHomeBase = false;
        GameControllerSingleton.instance.aliveCount = PhotonNetwork.CurrentRoom.PlayerCount;
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
