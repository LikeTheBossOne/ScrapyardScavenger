using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerControllerLoader : MonoBehaviourPun
{
    public GameObject playerController;
    public BaseDataManager baseDataManager;
    public InGameDataManager inGameDataManager;
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
                baseDataManager = obj.GetComponent<BaseDataManager>();
                inGameDataManager = obj.GetComponent<InGameDataManager>();
                sceneManager = obj.GetComponent<PlayerSceneManager>();
                skillManager = obj.GetComponent<SkillManager>();
            }
        }

        sceneManager.isInHomeBase = false;
        GameControllerSingleton.instance.aliveCount = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    void Start()
    {
        baseDataManager.gunParent = gunParent;
		inGameDataManager.gunParent = gunParent;
        baseDataManager.grenadeParent = grenadeParent;
        inGameDataManager.grenadeParent = grenadeParent;
        baseDataManager.medShotParent = medParent;
        inGameDataManager.medShotParent = medParent;
        baseDataManager.SetupInScene();
		playerController.GetPhotonView().RPC("EquipWeapon", RpcTarget.All, 0);
    }

    void Update()
    {
        
    }
}
