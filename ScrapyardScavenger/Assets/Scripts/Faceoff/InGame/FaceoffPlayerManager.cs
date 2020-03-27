using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class FaceoffPlayerManager : MonoBehaviourPun
{
    public List<GameObject> players;
    public List<List<GameObject>> playerTeams;

    public TeamGroup clientTeam;

    private void Start()
    {
        players = new List<GameObject>();

        playerTeams = new List<List<GameObject>> {new List<GameObject>(), new List<GameObject>()};

        clientTeam = TeamGroup.TeamNotAssigned;
    }

    public IEnumerable<GameObject> GetOtherPlayers()
    {
        return players.Where((p => !p.GetPhotonView().IsMine));
    }

    public IEnumerable<GameObject> GetTeammates(GameObject player, TeamGroup team)
    {
        return playerTeams[(int) team].Where((p => p != player));
    }

    public void Register(GameObject adding, TeamGroup team)
    {
        players.Add(adding);
        playerTeams[(int)team].Add(adding);
        adding.GetComponent<FaceoffDeath>().OnPlayerDeath += PlayerDied;

        // Make friendlies and enemies correct color
        if (team == clientTeam)
        {
            adding.GetComponentInChildren<MeshRenderer>().material.color = Color.blue;
        }
        else
        {
            adding.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
        }

    }

    public void PlayerDied(GameObject deadPlayer, TeamGroup team)
    {
        players.Remove(deadPlayer);
        playerTeams[(int) team].Remove(deadPlayer);

        if (playerTeams[0].Count == 0 || playerTeams[1].Count == 0)
        {
            TeamGroup winners = playerTeams[0].Count > 0 ? TeamGroup.TeamOne : TeamGroup.TeamTwo;
            NotificationType notifType =
                deadPlayer.GetPhotonView().IsMine ? NotificationType.Bad : NotificationType.Good;

            NotificationSystem.Instance.Notify(new Notification($"Team {(int)winners + 1} won!", notifType));
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(StopGame());
            }
        }

        
    }

    IEnumerator StopGame()
    {
        yield return new WaitForSeconds(3);

        PhotonNetwork.LoadLevel(3);
    }
}
