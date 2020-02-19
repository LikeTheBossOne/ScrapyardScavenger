using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill Level", menuName = "Skill Levels/SkillLevel")]
public class SkillLevel : ScriptableObject
{
    
    public Canvas Icon;
    public int XPNeeded;
    public bool IsUnlocked;
    public float Modifier; // really only used for health? but extensible for other attributes if we want

}
