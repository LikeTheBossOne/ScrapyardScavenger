using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Health : MonoBehaviourPunCallbacks
{
    public int maxHealth;
    public int currentHealth { get; private set; }

	public PlayerHUD pHud;

    void Start()
    {
        currentHealth = maxHealth;
		pHud = GetComponent<PlayerHUD>();
    }

    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        if (photonView.IsMine)
        {
			pHud.takeDamage((float) damage);
            currentHealth -= damage;
        }
    }

    public void Heal(int amount)
    {
        //TakeDamage(50);
        Debug.Log("About to heal, current health: " + currentHealth);
        currentHealth += amount;
        Debug.Log("Healed, current health: " + currentHealth);
        
    }
}
