using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : ScriptableObject
{

    public string Description;

    // list of levels for this skill
    public SkillLevel[] levels;

    public abstract void Unlock(SkillLevel unlockedLevel, SkillManager skillManager);


}
