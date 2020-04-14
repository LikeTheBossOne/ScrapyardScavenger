using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;

public class GameSetupController : MonoBehaviour
{
    public Vector3 player1StartPosition;
    public Vector3 player2StartPosition;

    void Start()
    {
        CreatePlayer();
    }

    private void CreatePlayer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonPlayer"), player1StartPosition, Quaternion.Euler(0, 90, 0));

            PhotonNetwork.Instantiate("PhotonPrefabs/BuyableManager", Vector3.zero, Quaternion.identity);
            PhotonNetwork.Instantiate("PhotonPrefabs/ZoneManagerPref", Vector3.zero, Quaternion.identity);
        }
        else
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonPlayer"), player2StartPosition, Quaternion.Euler(0, 90, 0));
        }
        
    }
}
