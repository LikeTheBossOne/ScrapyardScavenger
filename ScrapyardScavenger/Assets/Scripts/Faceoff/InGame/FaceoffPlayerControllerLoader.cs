using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class FaceoffPlayerControllerLoader : MonoBehaviourPun
{
    public GameObject playerController;
    public BaseDataManager baseDataManager;
    public InGameDataManager inGameDataManager;
    public PlayerSceneManager sceneManager;

    public Transform gunParent;
    public Transform meleeParent;
    public Transform grenadeParent;
    public Transform medParent;

    void Awake()
    {
        Debug.Log("PCAwakeLoader" + photonView.ViewID);
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameController");
        foreach (var obj in objs)
        {
            if (obj.GetPhotonView().Owner.UserId == photonView.Owner.UserId)
            {
                playerController = obj;
                baseDataManager = obj.GetComponent<BaseDataManager>();
                inGameDataManager = obj.GetComponent<InGameDataManager>();
                sceneManager = obj.GetComponent<PlayerSceneManager>();
            }
        }

        sceneManager.isInHomeBase = false;
        GameControllerSingleton.instance.aliveCount = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    void Start()
    {
        baseDataManager.gunParent = gunParent;
        inGameDataManager.gunParent = gunParent;
        baseDataManager.SetupInScene();
        playerController.GetPhotonView().RPC("EquipWeapon", RpcTarget.All, 0);
        Debug.Log("PCLoader" + photonView.ViewID);
    }

    void Update()
    {
    
    }
}
