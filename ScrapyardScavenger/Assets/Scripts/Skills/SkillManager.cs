using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// this is the in-game version that tracks what skills a player has
public class SkillManager : MonoBehaviourPunCallbacks
{
    private List<Skill> skills;
    private int skillIndex;
    private int tempXP; // this is the XP a player is currently collecting in the game
    private int finalXP; // this is the final XP number

    private bool twoMinuteFlag;

    private float TWO_MINUTES = 120f;
    private float FIVE_MINUTES = 300f;

    // Start is called before the first frame update
    void Start()
    {
        // initialize to have 0 skills
        skills = new List<Skill>();

        twoMinuteFlag = false;

        // initialize to have 0 XP
        tempXP = 0;
        finalXP = 0;
    }

    private bool IsMultipleOfFive(float seconds)
    {
        return (seconds >= FIVE_MINUTES && seconds % FIVE_MINUTES == 0);
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

            //Debug.Log("Temp XP: " + tempXP);
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

    public bool HasSkill(Skill skill)
    {
        return skills.Contains(skill);
    }

    public bool UnlockSkill(Skill skill) {
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
            return false;
        }

        Debug.Log("finalXP: " + finalXP + ", and thisLevel.XPNeeded: " + thisLevel.XPNeeded);
        if (finalXP < thisLevel.XPNeeded)
        {
            Debug.Log("Not enough XP to unlock this skill");
            return false;
        }

        // this call is for making sure each skill's effect takes place
        skill.Unlock(thisLevel, this);
        thisLevel.UnlockIcon();

        if (!HasSkill(skill))
        {
            // just add it
            skills.Add(skill);
        }
        else
        {
            // find it and then replace it
            for (int i = 0; i < skills.Count; i++)
            {
                if (skills[i].name == skill.name)
                {
                    skills[i] = skill;
                    break;
                }
            }
        }
        
        


        // spend the XP
        SpendXP(thisLevel.XPNeeded);
        return true;
    }

    
    
    // used for gaining XP in the game
    public void GainXP(int xpAmount)
    {
        tempXP += xpAmount;
        Debug.Log("Temp XP: " + tempXP);
    }

    // used for spending XP in the home base
    public bool SpendXP(int spendingAmount)
    {
        if (spendingAmount > finalXP)
        {
            return false;
        }
        Debug.Log("Final XP before spending: " + finalXP);
        finalXP -= spendingAmount;
        Debug.Log("Final XP after spending: " + finalXP);
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
}
