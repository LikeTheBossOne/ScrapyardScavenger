using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SkillManager : MonoBehaviourPun
{

    public Skill[] skills;
    private int skillIndex;
    public bool isOpen;

    private int currentXP;

    // Start is called before the first frame update
    void Start()
    {
        // no need to initialize since the 3 skills are assigned beforehand

        isOpen = false;

        skillIndex = 0;
        currentXP = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;

        // Check if player is opening/closing skill screen
        if (Input.GetKeyDown(KeyCode.X))
        {
            isOpen = !isOpen;
            if (isOpen)
                Debug.Log("Opened Skill Tree");
            else
                Debug.Log("Closed Skill Tree");
        }

        if (isOpen)
        {
            short changeSlot = 0;
            if (Input.GetKeyDown(KeyCode.V))
            {
                changeSlot = 1;
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                changeSlot = -1;
            }

            if (changeSlot != 0)
            {
                skillIndex += changeSlot;
                skillIndex = mod(skillIndex, skills.Length);
                Debug.Log($"Skill switched to {skills[skillIndex].name}");
            }

            // Upgrade selected skill
            if (Input.GetKeyDown(KeyCode.B))
            {
                UnlockSkill(skills[skillIndex]);
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                // add 100 XP
                currentXP += 100;
                Debug.Log("Current XP: " + currentXP);
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                // add 1,000 XP
                currentXP += 1000;
                Debug.Log("Current XP: " + currentXP);
            }
        }
    }

    public void UnlockSkill(Skill skill) {
        // check to see if player can unlock this skill/upgrade it
        SkillLevel thisLevel = null;
        bool canUnlock = false;
        for (int i = 0; i < skill.levels.Length; i++)
        {
            thisLevel = skill.levels[i];
            if (!thisLevel.IsUnlocked)
            {
                canUnlock = true;
                break;
            }
                
        }
        if (!canUnlock)
        {
            Debug.Log("Cannot unlock the selected skill");
            return;
        }

        Debug.Log("currentXP: " + currentXP + ", and thisLevel.XPNeeded: " + thisLevel.XPNeeded);
        if (currentXP < thisLevel.XPNeeded)
        {
            Debug.Log("Not enough XP to unlock this skill");
            return;
        }

        // this call is for making sure each skill's effect takes place
        skill.Unlock(thisLevel, this);

        // spend the XP
        SpendXP(thisLevel.XPNeeded);
    }

    public void GainXP(int xpAmount)
    {
        currentXP += xpAmount;
    }

    public bool SpendXP(int spendingAmount)
    {
        if (spendingAmount > currentXP)
        {
            return false;
        }
        Debug.Log("Current XP before spending: " + currentXP);
        currentXP -= spendingAmount;
        Debug.Log("Current XP after spending: " + currentXP);
        return true;
    }

    private int mod(int x, int m)
    {
        return (x % m + m) % m;
    }
}
