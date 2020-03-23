using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Skill that increases the maximum time a player can sprint
 */
[CreateAssetMenu(fileName = "New Skill", menuName = "Skills/Endurance")]
public class Endurance : Skill
{

    public override void UnlockLevel(int levelIndex, SkillManager skillManager)
    {
        // increase the length of time that a player can sprint
        levels[levelIndex].Unlock(levelIndex);
        HighestLevel = levelIndex;

        // comment this out for testing purposes with UI-integration
        /*PlayerMotor motor = skillManager.GetComponent<PlayerMotor>();
        motor.SetSprintLimit((int)unlockedLevel.Modifier);*/
    }

}
