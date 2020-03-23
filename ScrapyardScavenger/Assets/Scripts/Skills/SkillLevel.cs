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
    private Image SelectBorder;
    private Text RequiredXPText;
    private Image Skill_Icon;
    private Text Skill_Numeral;

    public void SetCanvas(GameObject obj)
    {
        Icon = obj;

        // go ahead and save the individual objects
        foreach (Text textObj in Icon.GetComponentsInChildren<Text>(true))
        {
            if (textObj.name == "Required XP")
            {
                RequiredXPText = textObj;
            }
            else if (textObj.name.EndsWith("Numeral"))
            {
                Skill_Numeral = textObj;
            }
        }

        foreach (Image imageObj in Icon.GetComponentsInChildren<Image>(true))
        {
            if (imageObj.name == "Selected Border")
            {
                SelectBorder = imageObj;
            }
            else if (imageObj.name.EndsWith("Icon"))
            {
                Skill_Icon = imageObj;
            }
        }
        
    }

    public void SelectIcon()
    {
        // this is selected with the cursor
        // so bring up the yellow thing
        SelectBorder.gameObject.SetActive(true);

        // and display the required XP if it's not unlocked
        if (!IsUnlocked)
        {
            RequiredXPText.gameObject.SetActive(true);
            RequiredXPText.text = XPNeeded + " XP";

        }
    }

    public void DeselectIcon()
    {
        SelectBorder.gameObject.SetActive(false);
        RequiredXPText.gameObject.SetActive(false);
    }

    public void Unlock(int levelIndex)
    {
        IsUnlocked = true;
        UnlockIcon();
    }

    public void UnlockIcon()
    {
        // set the icon and the numeral objects
        Image[] images = Icon.GetComponentsInChildren<Image>();
        Image skill_Icon = null;
        for (int i = 0; i < images.Length; i++)
        {
            if (images[i].name.EndsWith("Icon"))
            {
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
