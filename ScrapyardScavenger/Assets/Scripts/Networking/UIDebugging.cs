using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class UIDebugging : MonoBehaviour
{

    void Start()
    {
        GetComponent<Text>().text = PhotonNetwork.CloudRegion + PhotonNetwork.CurrentRoom.Name.Substring(4);
    }

    void Update()
    {
        
    }
}
