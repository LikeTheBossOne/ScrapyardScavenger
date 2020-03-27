using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this observes the skill tree UI and reacts to user input
public class SkillTreeObserver : MonoBehaviour
{
    // list of all the possible skills in the UI
    public Skill[] skills;
    private int skillIndex;
    public GameObject playerController;

    // Start is called before the first frame update
    void Start()
    {
        // initialize all levels of the 3 skills to be locked
        for (int i = 0; i < skills.Length; i++)
        {
            Skill currentLevel = skills[i];
            for (int j = 0; j < currentLevel.levels.Length; j++)
            {
                currentLevel.levels[j].IsUnlocked = false;
                string canvasName = currentLevel.levels[j].name + " Skill";
                Debug.Log("Canvas name: " + canvasName);
                currentLevel.levels[j].SetCanvas(GameObject.Find(canvasName));
            }
        }

        skillIndex = 0;
        playerController = GameObject.FindGameObjectsWithTag("GameController")[0];
    }

    // Update is called once per frame
    void Update()
    {
        short changeSlot = 0;
        if (Input.GetKeyDown(KeyCode.V))
        {
            changeSlot = 1;
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            changeSlot = -1;
        }

        if (changeSlot != 0)
        {
            skillIndex += changeSlot;
            skillIndex = mod(skillIndex, skills.Length);
            Debug.Log($"Skill switched to {skills[skillIndex].name}");

            // highlight the selected skill in the UI?
        }

        // Upgrade selected skill
        if (Input.GetKeyDown(KeyCode.B))
        {
            UnlockSkill(skills[skillIndex]);
        }

        
    }

    public void UnlockSkill(Skill skill)
    {
        // check to see if player can unlock this skill/upgrade it
        playerController.GetComponent<SkillManager>().UnlockSkill(skill);
    }

    private int mod(int x, int m)
    {
        return (x % m + m) % m;
    }

}
