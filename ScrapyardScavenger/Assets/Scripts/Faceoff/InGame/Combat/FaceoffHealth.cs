using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class FaceoffHealth : MonoBehaviourPun
{
    public int maxHealth;
    public int currentHealth { get; private set; }
    private float armorModifier = 1.0f;
    private float skillModifier = 1.0f;

    public FaceoffPlayerHUD pHud;
    public FaceoffDeath death;

    void Start()
    {
        currentHealth = (int)(maxHealth * skillModifier * armorModifier);
        pHud = GetComponent<FaceoffPlayerHUD>();
        death = GetComponent<FaceoffDeath>();
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

    public void TakeDamage(int damage, int actor)
    {
        if (photonView.IsMine)
        {
            pHud.takeDamage((float)damage);
            currentHealth -= damage;
        }
        if (currentHealth <= 0)
        {
            photonView.RPC("PlayerDied", RpcTarget.All, actor);
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
