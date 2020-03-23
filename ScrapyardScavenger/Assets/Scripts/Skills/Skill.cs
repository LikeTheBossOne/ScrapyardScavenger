using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : ScriptableObject
{

    public string Description;
    public int HighestLevel = -1;

    // list of levels for this skill
    public SkillLevel[] levels;

    public abstract void UnlockLevel(int levelIndex, SkillManager skillManager);

    public bool IsUnlocked()
    {
        return levels[0].IsUnlocked;
    }
}
