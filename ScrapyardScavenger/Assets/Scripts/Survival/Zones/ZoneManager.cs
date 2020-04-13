using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ZoneManager : MonoBehaviourPun
{

    private Zones CurrentZone; // what Zone the player is in

    // Start is called before the first frame update
    void Start()
    {
        CurrentZone = Zones.Zone1; // player should always start in Zone1
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Zones GetCurrentZone()
    {
        return CurrentZone;
    }

    [PunRPC]
    public void SetCurrentZone(int newZone)
    {
        if (photonView.IsMine)
        {
            CurrentZone = (Zones)newZone;
            Debug.Log("Current Zone: " + newZone);
        }
    }
}
