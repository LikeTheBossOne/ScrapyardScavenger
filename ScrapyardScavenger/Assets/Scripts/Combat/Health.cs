using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Health : MonoBehaviourPunCallbacks
{
    public int maxHealth;
    public int currentHealth { get; private set; }
    private float armorModifier = 1.0f;
    private float skillModifier = 1.0f;

	public PlayerHUD pHud;
    public Death death;

    void Start()
    {
        currentHealth = (int) (maxHealth * skillModifier * armorModifier);
		pHud = GetComponent<PlayerHUD>();
        death = GetComponent<Death>();
    }

    void Update()
    {
        
    }

    public void ChangeHealthSkill(float modifier)
    {
        skillModifier = modifier;
    }

    public void ChangeHealthArmor(float modifier)
    {
        armorModifier = modifier;
    }

    public void TakeDamage(int damage)
    {
        if (photonView.IsMine)
        {
			pHud.takeDamage((float) damage);
            currentHealth -= damage;
        }
        if (currentHealth <= 0)
        {
            death.hasDied();
        }
    }

    public void Heal(int amount)
    {
        if (photonView.IsMine)
        {
            Debug.Log("About to heal, current health: " + currentHealth);
            currentHealth += amount;
            Debug.Log("Healed, current health: " + currentHealth);
        }
        
    }
}
