using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Health : MonoBehaviourPunCallbacks
{
    public int maxHealth;
    public int currentHealth { get; private set; }

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        if (photonView.IsMine)
        {
            currentHealth -= damage;
            Debug.Log(currentHealth);
        }
    }

    public void Heal()
    {
        TakeDamage(50);
        Debug.Log("About to heal, current health: " + currentHealth);
        currentHealth = maxHealth;
        Debug.Log("Healed, current health: " + currentHealth);
        
    }
}
