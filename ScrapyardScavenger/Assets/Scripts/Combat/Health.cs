using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Health : MonoBehaviourPunCallbacks
{
    public int maxHealth;
    public int currentHealth { get; private set; }
    private float skillModifier = 1.0f;

	public PlayerHUD pHud;
    public Death death;
    public InGameDataManager dataManager;

    void Start()
    {
        currentHealth = (int) (maxHealth * skillModifier);
		pHud = GetComponent<PlayerHUD>();
        death = GetComponent<Death>();
        if (photonView.IsMine)
            dataManager = GetComponent<PlayerControllerLoader>().inGameDataManager;
    }

    void Update()
    {
        
    }

    public void ChangeHealthSkill(float modifier)
    {
        skillModifier = modifier;
    }

    public void TakeDamage(int damage)
    {
        if (photonView.IsMine)
        {
            float armorMultiplier = dataManager.currentArmor != null ? dataManager.currentArmor.damageMultiplier : 1f;
			pHud.takeDamage((float) damage * armorMultiplier);
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                photonView.RPC("PlayerDied", RpcTarget.All);
            }
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
