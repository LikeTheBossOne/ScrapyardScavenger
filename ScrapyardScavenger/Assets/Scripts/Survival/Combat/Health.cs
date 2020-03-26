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
        
		pHud = GetComponent<PlayerHUD>();
        death = GetComponent<Death>();

        // see if the player has the Resilience skill
        SkillLevel resilienceLevel = GetComponent<PlayerControllerLoader>().skillManager.GetSkillByName("Resilience");
        if (resilienceLevel != null)
        {
            // they have it, so set it
            skillModifier = resilienceLevel.Modifier;
            Debug.Log("Player has Resilience, modifier is: " + skillModifier);
        }
        else Debug.Log("Player does NOT have Resilience");

        currentHealth = (int)(maxHealth * skillModifier * armorModifier);
        Debug.Log("Player is starting with current health: " + currentHealth); // this may cause problems when we implement the medpack
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
            photonView.RPC("PlayerDied", RpcTarget.All);
        }
    }

    public void Heal(int amount)
    {
        if (photonView.IsMine)
        {
            currentHealth += amount;
        }
        
    }
}
