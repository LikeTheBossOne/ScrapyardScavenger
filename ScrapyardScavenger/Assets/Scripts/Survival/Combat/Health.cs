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

		pHud = GetComponent<PlayerHUD>();
        death = GetComponent<Death>();
        if (photonView.IsMine)
            dataManager = GetComponent<PlayerControllerLoader>().inGameDataManager;

        // see if the player has the Resilience skill
        SkillLevel resilienceLevel = GetComponent<PlayerControllerLoader>().skillManager.GetSkillByName("Resilience");
        if (resilienceLevel != null)
        {
            // they have it, so set it
            skillModifier = resilienceLevel.Modifier;
            Debug.Log("Player has Resilience, modifier is: " + skillModifier);
        }
        else Debug.Log("Player does NOT have Resilience");

        currentHealth = (int) (maxHealth * skillModifier);
        Debug.Log("Player is starting with current health: " + currentHealth); // this may cause problems when we implement the medpack
    }

    void Update()
    {

    }

    public void ChangeHealthSkill(float modifier)
    {
        skillModifier = modifier;
    }

    public void TakeDamage(int baseDamage)
    {
        if (photonView.IsMine)
        {
            float armorMultiplier = dataManager.currentArmor != null ? dataManager.currentArmor.damageMultiplier : 1f;
            float totalDamage = baseDamage * armorMultiplier;

			pHud.takeDamage(totalDamage);
            currentHealth -= (int)Mathf.Floor(totalDamage);
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
