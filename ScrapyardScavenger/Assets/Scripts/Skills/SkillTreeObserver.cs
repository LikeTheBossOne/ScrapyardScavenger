using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// this observes the skill tree UI and reacts to user input
public class SkillTreeObserver : MonoBehaviour
{
    // list of all the possible skills in the UI
    private List<Skill> displaySkills;
    private int skillIndex;
    private int levelIndex;
    public GameObject playerController;
    public Text playerXPText;

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
            for (int j = 0; j < currentSkill.levels.Length; j++)
            {
                string canvasName = currentSkill.levels[j].name + " Skill";
                currentSkill.levels[j].SetCanvas(GameObject.Find(canvasName));

                if (currentSkill.levels[j].IsUnlocked)
                {
                    currentSkill.levels[j].UnlockIcon();
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        short changeSkillSlot = 0;
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            changeSkillSlot = 1;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            changeSkillSlot = -1;
        }

        if (changeSkillSlot != 0)
        {
            displaySkills[skillIndex].levels[levelIndex].DeselectIcon();
            skillIndex += changeSkillSlot;
            skillIndex = mod(skillIndex, displaySkills.Count);
            Debug.Log($"Skill switched to {displaySkills[skillIndex].levels[levelIndex].name}");

            // highlight the selected skill in the UI?
            displaySkills[skillIndex].levels[levelIndex].SelectIcon();
        }

        // check to see if they are going up or down
        short changeLevelSlot = 0;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            changeLevelSlot = 1;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            changeLevelSlot = -1;
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
        if (Input.GetKeyDown(KeyCode.B))
        {
            // check to see if player can unlock this skill/upgrade it
            if (playerController.GetComponent<SkillManager>().UnlockSkill(skillIndex, levelIndex))
            {
                UpdateXPInUI();
            }
            
        }

        
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
