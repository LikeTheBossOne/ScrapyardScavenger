using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// this is the in-game version that tracks what skills a player has
public class SkillManager : MonoBehaviourPunCallbacks
{
    public List<Skill> skills;
    private int skillIndex;
    private int tempXP; // this is the XP a player is currently collecting in the game
    private int finalXP; // this is the final XP number

    private bool twoMinuteFlag;

    private float TWO_MINUTES = 120f;
    private float FIVE_MINUTES = 300f;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize all the skills (to make sure previous gamedata doesn't carry over
        InitializeSkills();

        twoMinuteFlag = false;

        // initialize to have 0 XP
        tempXP = 0;
        finalXP = 0;
    }

    private void InitializeSkills()
    {
        foreach (Skill skill in skills)
        {
            skill.HighestLevel = -1;
            foreach (SkillLevel level in skill.levels)
            {
                level.IsUnlocked = false;
            }
        }
    }

    private bool IsMultipleOfFive(float seconds)
    {
        return (seconds >= FIVE_MINUTES && seconds % FIVE_MINUTES == 0);
    }

    public bool HasAnySkills()
    {
        foreach (Skill skill in skills)
        {
            if (skill.IsUnlocked())
            {
                return true;
            }
        }
        return false;
    }

    public List<Skill> GetSkills()
    {
        return skills;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;

        if (!GetComponent<PlayerSceneManager>().isInHomeBase)
        {
            // check to see how long the player has been in the game
            if (!twoMinuteFlag && Time.timeSinceLevelLoad >= TWO_MINUTES)
            {
                // gain xp for being in the game for 2 minutes
                Debug.Log("Two minutes have passed");
                GainXP((int) XPRewards.TwoMinutes);
                twoMinuteFlag = true;
            }
            if (IsMultipleOfFive(Time.timeSinceLevelLoad))
            {
                Debug.Log("Five minutes have passed");
                GainXP((int)XPRewards.FiveMinutes);
            }
        }
        

        // only used for testing purposes
        if (Input.GetKeyDown(KeyCode.N))
        {
            // add 100 XP
            tempXP += 100;
            Debug.Log("Temp XP: " + tempXP);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            // add 1,000 XP
            tempXP += 1000;
            Debug.Log("Temp XP: " + tempXP);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            // add 1,000 XP
            Debug.Log("Final XP: " + finalXP);
            Debug.Log("Temp XP: " + tempXP);
        }
    }

    public bool UnlockSkill(int skillIndex, int levelIndex) {
        // first, check to see if the player already has this skill
        if (skills[skillIndex].levels[levelIndex].IsUnlocked)
        {
            Debug.Log("Skill is already unlocked");
            return false;
        }

        // check to see if player can unlock this skill/upgrade it
        bool canUnlock = true;

        // basically check to see if the levels before it are unlocked
        // use HighestLevel
        Debug.Log("Highest level: " + skills[skillIndex].HighestLevel);
        if (skills[skillIndex].HighestLevel + 1 != levelIndex)
        {
            canUnlock = false;
        }

        if (!canUnlock)
        {
            Debug.Log("Cannot unlock the selected skill level");
            return false;
        }

        // check to see if the player has enough XP to unlock this level
        if (finalXP < skills[skillIndex].levels[levelIndex].XPNeeded)
        {
            Debug.Log("Not enough XP to unlock this skill level");
            return false;
        }

        // this call is for making sure each skill's effect takes place
        skills[skillIndex].UnlockLevel(levelIndex, this);

        // spend the XP
        SpendXP(skills[skillIndex].levels[levelIndex].XPNeeded);
        return true;
    }

    
    
    // used for gaining XP in the game
    public void GainXP(int xpAmount)
    {
        tempXP += xpAmount;
    }

    // used for spending XP in the home base
    public bool SpendXP(int spendingAmount)
    {
        if (spendingAmount > finalXP)
        {
            return false;
        }
        finalXP -= spendingAmount;
        return true;
    }

    // used if a player successfully makes it back to the home base
    public void TransferXP()
    {
        finalXP += tempXP;
        tempXP = 0;
    }

    // used if a player dies
    public void ClearTempXP()
    {
        tempXP = 0;
    }

    public float GetFinalXP()
    {
        return finalXP;
    }
}
