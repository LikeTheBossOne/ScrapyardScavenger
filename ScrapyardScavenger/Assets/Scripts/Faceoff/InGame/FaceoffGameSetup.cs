using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class FaceoffGameSetup : MonoBehaviour
{
    public GameObject team1Spawn;
    public GameObject team2Spawn;

    void Start()
    {
        CreatePlayer();
    }

    private void CreatePlayer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;

            if (PhotonNetwork.CurrentRoom.PlayerCount < 3)
            {
                int i = 0;
                foreach (var player in players)
                {
                    Vector3 spawn = i % 2 == 0
                        ? team1Spawn.transform.GetChild((i + 1) / 2).position
                        : team2Spawn.transform.GetChild((i + 1) / 2).position;
                    Vector3 rotation = i % 2 == 0
                        ? new Vector3(0, 90, 0)
                        : new Vector3(0, -90, 0);

                    GameObject p = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonPlayer"), spawn, Quaternion.Euler(rotation));
                    p.GetPhotonView().TransferOwnership(player.Key);

                    i++;
                }
            }
        }
    }
}
