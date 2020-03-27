using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameControllerSingleton : MonoBehaviourPunCallbacks
{

    #region singleton

    public static GameControllerSingleton instance = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    #endregion

    public int aliveCount;

    void Start()
    {
        aliveCount = 0;
    }
}
