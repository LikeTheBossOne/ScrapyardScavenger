using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Stats : MonoBehaviour
{
    public enum Condition{
        normal
    }
    public int baseHealth;
    protected int health; // consider making health a float
    public int status;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetCurrentHealth()
    {
        return health;
    }

    [PunRPC]
    public void ModifyHealth(float modifier)
    {
        health = (int) (health * modifier);
    }

    [PunRPC]
    protected void TakeDamage(int damage)
    {
        if (gameObject.GetPhotonView().IsMine)
        {
            //, GameObject damager, int atkStatus
            health = health - damage;
            /*if (atkStatus > 0)
            {
                status = atkStatus;
            }*/
        }

    }
}
