using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

// this observes the skill tree UI and reacts to user input
public class SkillTreeObserver : MonoBehaviour
{
    // list of all the possible skills in the UI
    private List<Skill> displaySkills;
    private int skillIndex;
    private int levelIndex;
    public GameObject playerController;
    public Text playerXPText;

	public GameObject backButton;

	private float delay = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        // check to see if the player has any skills...
        playerController = GameObject.FindGameObjectsWithTag("GameController")[0];
        displaySkills = playerController.GetComponent<SkillManager>().skills;
        InitializeSkills();

        skillIndex = 0;
        levelIndex = 0;
        
        UpdateXPInUI();
    }

    private void InitializeSkills()
    {
        // initialize all levels of the 3 skills to be locked
        for (int i = 0; i < displaySkills.Count; i++)
        {
            Skill currentSkill = displaySkills[i];
            
            string rootCanvasName = currentSkill.name + " Skills";
            foreach (Component comp in GameObject.Find(rootCanvasName).GetComponentsInChildren<Canvas>())
            {
                // find the level with this name, just to make it more extensible than hardcoding three levels
                for (int j = 0; j < currentSkill.levels.Length; j++)
                {
                    if ("Level " + currentSkill.levels[j].Level_Name == comp.name)
                    {
                        
                        currentSkill.levels[j].SetCanvas(comp.gameObject);
                        currentSkill.levels[j].SetSkillDescription(currentSkill.Description);
                        if (currentSkill.levels[j].IsUnlocked) currentSkill.levels[j].UnlockIcon();
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        short changeSkillSlot = 0;
		if (Input.GetKeyDown(KeyCode.RightArrow) || (Input.GetAxis("Horizontal") == 1 && Time.time > delay))
        {
            changeSkillSlot = 1;
			delay = Time.time + 0.3f;
        }
		else if (Input.GetKeyDown(KeyCode.LeftArrow) || (Input.GetAxis("Horizontal") == -1 && Time.time > delay))
        {
            changeSkillSlot = -1;
			delay = Time.time + 0.3f;
        }

        if (changeSkillSlot != 0)
        {
            displaySkills[skillIndex].levels[levelIndex].DeselectIcon();
            skillIndex += changeSkillSlot;
            skillIndex = mod(skillIndex, displaySkills.Count);
            //Debug.Log($"Skill switched to {displaySkills[skillIndex].levels[levelIndex].name}");

            // highlight the selected skill in the UI?
            displaySkills[skillIndex].levels[levelIndex].SelectIcon();
        }

        // check to see if they are going up or down
        short changeLevelSlot = 0;
		if (Input.GetKeyDown(KeyCode.UpArrow) || (Input.GetAxis("Vertical") == 1 && Time.time > delay) )
        {
            changeLevelSlot = 1;
			delay = Time.time + 0.3f;
        }
		else if (Input.GetKeyDown(KeyCode.DownArrow) || (Input.GetAxis("Vertical") == -1 && Time.time > delay) )
        {
            changeLevelSlot = -1;
			delay = Time.time + 0.3f;
        }

        if (changeLevelSlot != 0)
        {
            displaySkills[skillIndex].levels[levelIndex].DeselectIcon();
            levelIndex += changeLevelSlot;
            levelIndex = mod(levelIndex, displaySkills[skillIndex].levels.Length);
            Debug.Log($"Skill switched to {displaySkills[skillIndex].levels[levelIndex].name}");

            // highlight the selected skill in the UI?
            displaySkills[skillIndex].levels[levelIndex].SelectIcon();
        }


        // Upgrade selected skill
		if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown("joystick button 0") )
        {
            AttemptUnlock();
		} else if (Input.GetKeyDown("joystick button 0")) {
			AttemptUnlock();
		} else if (Input.GetKeyDown("joystick button 1")) {
			backButton.GetComponent<Button>().onClick.Invoke();
		}
        
    }

    public void AttemptUnlock()
    {
        // check to see if player can unlock this skill/upgrade it
        if (playerController.GetComponent<SkillManager>().UnlockSkill(skillIndex, levelIndex))
        {
            UpdateXPInUI();
        }
    }

    public void ClickSkill(int si, int li)
    {
        displaySkills[skillIndex].levels[levelIndex].DeselectIcon();

        skillIndex = si;
        levelIndex = li;

        displaySkills[skillIndex].levels[levelIndex].SelectIcon();
    }

    private void UpdateXPInUI()
    {
        // get the player's XP
        float xp = playerController.GetComponent<SkillManager>().GetFinalXP();

        // update the Text
        playerXPText.text = "Your XP: " + xp;
    }

    public void ResetCursor()
    {
        displaySkills[skillIndex].levels[levelIndex].DeselectIcon();
        skillIndex = 0;
        levelIndex = 0;
    }

    private int mod(int x, int m)
    {
        return (x % m + m) % m;
    }

}
