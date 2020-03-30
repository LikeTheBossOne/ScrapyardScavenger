using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Skill that increases the drop rate of resources
 */
[CreateAssetMenu(fileName = "New Skill", menuName = "Skills/Scavenger")]
public class Scavenger : Skill
{

    public override void UnlockLevel(int levelIndex, SkillManager skillManager)
    {
        // increase the chance of collecting 2 resources instead of 1
        levels[levelIndex].Unlock(levelIndex);
        HighestLevel = levelIndex;

    }

}
