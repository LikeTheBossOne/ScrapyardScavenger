using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ZombieAnimUtilities : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
   // void Update()
   // {
        
   // }

    public void Die()
    {
        gameObject.GetComponentInParent<Transform>().gameObject.GetPhotonView().RPC("CleanUp", RpcTarget.All);
    }
}
