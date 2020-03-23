using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Skill that increases the player's maximum health
 */
[CreateAssetMenu(fileName = "New Skill", menuName = "Skills/Resilience")]
public class Resilience : Skill
{

    public override void UnlockLevel(int levelIndex, SkillManager skillManager)
    {
        // increase the max health through a modifier
        levels[levelIndex].Unlock(levelIndex);
        HighestLevel = levelIndex;

        // comment this out for testing purposes with UI-integration
        /*Health health = skillManager.GetComponent<Health>();
        health.ChangeHealthSkill(unlockedLevel.Modifier);*/

    }

}
