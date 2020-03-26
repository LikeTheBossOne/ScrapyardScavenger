using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class ExampleSkillToggle : MonoBehaviour
{

    Toggle m_Toggle;
    public Canvas skill_Canvas;
    private Image skill_Icon;
    private Text skill_Numeral;


    // Start is called before the first frame update
    void Start()
    {
        //Fetch the Toggle GameObject
        m_Toggle = GetComponent<Toggle>();

        //Add listener for when the state of the Toggle changes, to take action
        m_Toggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged(m_Toggle);
        });

        // set the icon and the numeral objects
        Image[] images = skill_Canvas.GetComponentsInChildren<Image>();
        Debug.Log("Images length: " + images.Length);
        for (int i = 0; i < images.Length; i++)
        {
            if (images[i].name.EndsWith("Icon"))
            {
                Debug.Log("Found the icon");
                skill_Icon = images[i];
                break;
            }
        }
        skill_Numeral = skill_Canvas.GetComponentInChildren<Text>();
    }

    //Output the new state of the Toggle into Text
    void ToggleValueChanged(Toggle change)
    {
        Debug.Log("Toggled");
        // what to do with function argument 'change'?
        float alpha = 0f;

        // if on, set alpha to full (100% or 1f)
        if (m_Toggle.isOn)
        {
            Debug.Log("It's ON");
            alpha = 1f;
        }
        else
        {
            // set alpha to (20% or 0.2f)
            Debug.Log("It's OFF");
            alpha = 0.2f;
        }
        skill_Icon.color = new Color(skill_Icon.color.r, skill_Icon.color.g, skill_Icon.color.b, alpha);
        skill_Numeral.color = new Color(skill_Numeral.color.r, skill_Numeral.color.g, skill_Numeral.color.b, alpha);
    }
}
