using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Buyablestarter : MonoBehaviour
{
    void Start()
    {
        PersistentBuyableManager.Instance.gameObject.GetPhotonView().RPC("PlaceBuyables", RpcTarget.All);
    }

}
