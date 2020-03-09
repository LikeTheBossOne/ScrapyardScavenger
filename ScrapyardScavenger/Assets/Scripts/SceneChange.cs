using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SceneChange : MonoBehaviourPun
{
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
