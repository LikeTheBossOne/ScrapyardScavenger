using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Skill Level", menuName = "Skill Levels/SkillLevel")]
public class SkillLevel : ScriptableObject
{
    //public string canvasName;
    private GameObject Level_Canvas;
    public string Level_Name; // I, II, or III
    public string Skill_Effect; // effect for this specific skill level
    public int XPNeeded;
    public bool IsUnlocked;
    public float Modifier; // really only used for health? but extensible for other attributes if we want

    private string Skill_Description;
    private Image SelectBorder; // not popup
    private Image Skill_Icon; // not popup
    private Text Skill_Numeral; // not popup

    private Canvas Skill_Popup; // popup window
    private Text Required_XP_Text; // popup window
    private Text Description_Text; // popup
    private Text Effect_Text; // popup
    private Text Title_Text; // popup
    private Text Instructions_Text; // popup
    

    public void SetCanvas(GameObject obj)
    {
        Level_Canvas = obj;

        // go ahead and save the individual objects
        foreach (Text textObj in Level_Canvas.GetComponentsInChildren<Text>(true))
        {
            if (textObj.name == ("Numeral"))
            {
                Skill_Numeral = textObj;
            }
        }

        foreach (Image imageObj in Level_Canvas.GetComponentsInChildren<Image>(true))
        {
            if (imageObj.name == "Selected Border")
            {
                SelectBorder = imageObj;
            }
            else if (imageObj.name == "Icon")
            {
                Skill_Icon = imageObj;
            }
        }

        // save all the objects related to the popup window
        foreach (Canvas canv in Level_Canvas.GetComponentsInChildren<Canvas>(true))
        {
            if (canv.name == "Skill Popup")
            {
                // this is the popup window
                Debug.Log("Found the popup window");
                Skill_Popup = canv;
                foreach (Text textObj in Skill_Popup.GetComponentsInChildren<Text>())
                {
                    Debug.Log("Text name: " + textObj.name);
                    if (textObj.name == "Skill Title") Title_Text = textObj;
                    else if (textObj.name == "Skill Description") Description_Text = textObj;
                    else if (textObj.name == "Level Effect") Effect_Text = textObj;
                    else if (textObj.name == "Required XP") Required_XP_Text = textObj;
                    else if (textObj.name == "Instructions") Instructions_Text = textObj;
                }
            }
        }
        

        
    }

    public void SetSkillDescription(string desc)
    {
        Skill_Description = desc;
    }

    private void SetPopupText()
    {
        // try to set the text values for all the popup window stuff
        Title_Text.text = name;
        Description_Text.text = Skill_Description;
        Effect_Text.text = Skill_Effect;
        
        if (!IsUnlocked) {
            Required_XP_Text.text = XPNeeded + " XP";
            Instructions_Text.text = "Press B to unlock!";
        }
        else
        {
            Required_XP_Text.text = "Unlocked!";
            Instructions_Text.text = "";
        }
    }

    public void SelectIcon()
    {
        // this is selected with the cursor
        // so bring up the yellow thing
        SelectBorder.gameObject.SetActive(true);

        // and display the popup window
        Skill_Popup.gameObject.SetActive(true);
        SetPopupText();


        /*if (!IsUnlocked)
        {
            Required_XP_Text.gameObject.SetActive(true);
            Required_XP_Text.text = XPNeeded + " XP";

        }*/
    }

    public void DeselectIcon()
    {
        SelectBorder.gameObject.SetActive(false);
        Skill_Popup.gameObject.SetActive(false);
    }

    public void Unlock(int levelIndex)
    {
        IsUnlocked = true;
        UnlockIcon();
        SetPopupText();
    }

    public void UnlockIcon()
    {
        // set the icon and the numeral objects
        /*Image[] images = Level_Canvas.GetComponentsInChildren<Image>();
        Image skill_Icon = null;
        for (int i = 0; i < images.Length; i++)
        {
            if (images[i].name == "Icon")
            {
                skill_Icon = images[i];
                break;
            }
        }*/
        float alpha = 1.0f;
        //Text skill_Numeral = Level_Canvas.GetComponentInChildren<Text>();
        Skill_Icon.color = new Color(Skill_Icon.color.r, Skill_Icon.color.g, Skill_Icon.color.b, alpha);
        Skill_Numeral.color = new Color(Skill_Numeral.color.r, Skill_Numeral.color.g, Skill_Numeral.color.b, alpha);
    }

}
