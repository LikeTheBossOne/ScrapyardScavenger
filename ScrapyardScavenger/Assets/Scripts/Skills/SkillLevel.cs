using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Skill Level", menuName = "Skill Levels/SkillLevel")]
public class SkillLevel : ScriptableObject
{
    //public string canvasName;
    private GameObject Icon;
    public int XPNeeded;
    public bool IsUnlocked;
    public float Modifier; // really only used for health? but extensible for other attributes if we want

    public void SetCanvas(GameObject obj)
    {
        Debug.Log("Setting canvas for skill " + name + ", Icon: " + obj.name);
        Icon = obj;
    }

    public void UnlockIcon()
    {
        // set the icon and the numeral objects
        Image[] images = Icon.GetComponentsInChildren<Image>();
        Debug.Log("Images length: " + images.Length);
        Image skill_Icon = null;
        for (int i = 0; i < images.Length; i++)
        {
            if (images[i].name.EndsWith("Icon"))
            {
                Debug.Log("Found the icon");
                skill_Icon = images[i];
                break;
            }
        }
        float alpha = 1.0f;
        Text skill_Numeral = Icon.GetComponentInChildren<Text>();
        skill_Icon.color = new Color(skill_Icon.color.r, skill_Icon.color.g, skill_Icon.color.b, alpha);
        skill_Numeral.color = new Color(skill_Numeral.color.r, skill_Numeral.color.g, skill_Numeral.color.b, alpha);
    }

}
