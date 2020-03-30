using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class InGamePlayerManager : MonoBehaviour
{
    public List<GameObject> players;

    private void Awake()
    {
        players = new List<GameObject>();
    }

    public GameObject GetOtherPlayer()
    {
        foreach (var player in players)
        {
            if (!player.GetPhotonView().IsMine)
            {
                return player;
            }
        }

        throw new Exception("You asked for another player when another player does not exist!");
    }

    public void Register(GameObject adding)
    {
        players.Add(adding);
        adding.GetComponent<Death>().OnPlayerDeath += PlayerDied;
    }


    public void PlayerDied(GameObject deadPlayer)
    {
        players.Remove(deadPlayer);
    }
}
