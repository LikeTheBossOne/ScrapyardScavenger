using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerSceneManager : MonoBehaviourPun
{
    public bool isInHomeBase;
    public bool isInSkillMenu;

    void Start()
    {
        isInHomeBase = true;
        isInSkillMenu = false;
    }


    [PunRPC]
    public void MasterClientGoToHomeBase()
    {
        isInHomeBase = true;
//		if (photonView.IsMine) {
//            GetComponent<SkillManager>().TransferXP();
//			GetComponent<InGameDataManager>().TransferToStorage();
//		}
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }
}
