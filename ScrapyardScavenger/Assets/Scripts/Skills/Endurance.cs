using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Skill that increases the maximum time a player can sprint
 */
[CreateAssetMenu(fileName = "New Skill", menuName = "Skills/Endurance")]
public class Endurance : Skill
{

    public override void Unlock(SkillLevel unlockedLevel, SkillManager skillManager)
    {
        // increase the length of time that a player can sprint
        // logic here
        unlockedLevel.IsUnlocked = true;
    }

}
