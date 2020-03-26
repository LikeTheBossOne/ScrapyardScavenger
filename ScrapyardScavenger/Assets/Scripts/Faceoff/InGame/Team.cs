using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Team : MonoBehaviourPun
{
    public TeamGroup team;

    void Awake()
    {
        team = TeamGroup.TeamNotAssigned;
    }

    [PunRPC]
    void SetTeam(int newTeam)
    {
        team = (TeamGroup) newTeam;
        FindObjectOfType<FaceoffPlayerManager>().Register(gameObject, GetComponent<Team>().team);
    }
}

public enum TeamGroup
{
    TeamOne,
    TeamTwo,
    TeamNotAssigned,
}
