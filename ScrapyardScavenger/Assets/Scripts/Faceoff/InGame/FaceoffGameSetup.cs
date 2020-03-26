using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;

public class FaceoffGameSetup : MonoBehaviour
{
    public GameObject team1Spawn;
    public GameObject team2Spawn;

    void Start()
    {
        CreatePlayers();
    }

    private void CreatePlayers()
    {
        int pNum = PhotonNetwork.LocalPlayer.ActorNumber - 1;

        FaceoffPlayerManager playerManager = FindObjectOfType<FaceoffPlayerManager>();
        playerManager.clientTeam = (TeamGroup) (pNum % 2);

        Vector3 spawn = pNum % 2 == 0
            ? team1Spawn.transform.GetChild(pNum / 2).position
            : team2Spawn.transform.GetChild(pNum / 2).position;
        Vector3 rotation = pNum % 2 == 0
            ? new Vector3(0, 90, 0)
            : new Vector3(0, -90, 0);

        GameObject p = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "FaceoffPlayer"), spawn, Quaternion.Euler(rotation));
        p.GetPhotonView().RPC("SetTeam", RpcTarget.All, pNum % 2);
    }
}
