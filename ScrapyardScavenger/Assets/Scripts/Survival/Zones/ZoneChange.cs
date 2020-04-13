using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ZoneChange : MonoBehaviourPun
{

    private GameObject Ref1; // first object used as a reference for direction when leaving trigger
    private GameObject Ref2;
    public GameObject Spawner;

    // Start is called before the first frame update
    void Start()
    {
        Ref1 = gameObject.transform.Find("Ref1").gameObject;
        Ref2 = gameObject.transform.Find("Ref2").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            float PlayerToRef1Distance = Vector3.Distance(other.transform.parent.gameObject.transform.position, Ref1.transform.position);
            float PlayerToRef2Distance = Vector3.Distance(other.transform.parent.gameObject.transform.position, Ref2.transform.position);
            int zoneInt;
            if (PlayerToRef1Distance < PlayerToRef2Distance)
            {
                // the player is closer to ref1
                // so change zones based off of what Ref1's zone is
                zoneInt = (int)Ref1.GetComponent<ZoneReference>().Zone;
            }
            else
            {
                // player is closer to ref2
                zoneInt = (int)Ref2.GetComponent<ZoneReference>().Zone;
            }
            other.transform.parent.gameObject.GetPhotonView().RPC("SetCurrentZone", RpcTarget.All, zoneInt);
            Spawner.GetPhotonView().RPC("UpdateActiveZones", RpcTarget.All);
        }
    }
}
