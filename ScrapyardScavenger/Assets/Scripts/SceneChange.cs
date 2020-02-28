using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SceneChange : MonoBehaviourPun
{
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        if (photonView.IsMine)
            GameObject.Find("UIObserver").GetComponent<HomeBaseNetworkManager>().playerController = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
