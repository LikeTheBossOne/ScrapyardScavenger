using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShamblerStats : Stats
{
    
    public float damage { get; private set; }
    
    // Start is called before the first frame update
    private void OnEnable()
    {
        health = 10;
        damage = 10;
        status = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    [PunRPC]
    new void TakeDamage(int damage)
    {
        //, GameObject damager, int atkStatus
        //note GameObjects can be passed by RPC
        health = health - damage;
        Debug.Log("Enemy Damaged");
        /*if (atkStatus > 0)
        {
            status = atkStatus;
        }*/
        //GetComponentInParent<ShamblerDetection>().gotShot(damager);
    }
}
