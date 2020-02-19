using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Stats : MonoBehaviour
{
    public enum Condition{
        normal,
    }
    public int maxHealth;
    public int health;
    public int status;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [PunRPC]
    protected void TakeDamage(int damage, GameObject damager, int atkStatus)
    {
        health = health - damage;
        if (atkStatus > 0)
        {
            status = atkStatus;
        }
    }
}
