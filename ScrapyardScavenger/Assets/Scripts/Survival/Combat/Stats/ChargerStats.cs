using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChargerStats : Stats
{
    public int damage; // { get; private set; }
    
    // Start is called before the first frame update
    void Start()
    {
        health = 200;
        damage = 20;
        status = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    protected new void TakeDamage(int damage, GameObject damager, int atkStatus)
    {
        
        health = health - damage;
        if (atkStatus > 0)
        {
            status = atkStatus;
        }
        GetComponentInParent<ChargerDetection>().onDamage(damage, damager, atkStatus);
    }
}
