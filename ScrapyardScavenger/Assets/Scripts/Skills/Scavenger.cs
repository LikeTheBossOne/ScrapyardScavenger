using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Skill that increases the drop rate of resources
 */
[CreateAssetMenu(fileName = "New Skill", menuName = "Skills/Scavenger")]
public class Scavenger : Skill
{

    public override void Unlock(SkillLevel unlockedLevel, SkillManager skillManager)
    {
        // increase the drop rate of resources somehow
        // logic here
        unlockedLevel.IsUnlocked = true;
    }

}
