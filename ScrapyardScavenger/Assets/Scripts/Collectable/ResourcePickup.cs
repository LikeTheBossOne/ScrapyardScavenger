using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ResourcePickup : MonoBehaviour
{
    public ResourceType type;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // If either player is within certain radius of this prefab, destroy this from map and add it to the inventory
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(this.type);
            // before adding to the count, check the player's Scavenger skill
            SkillManager skillManager = other.transform.parent.GetComponent<SkillManager>();
            Skill scavengerSkill = null;
            for (int i = 0; i < skillManager.skills.Length; i++)
            {
                if (skillManager.skills[i].name == "Scavenger")
                {
                    // this is the Scavenger skill, so check to see which is the highest level
                    scavengerSkill = skillManager.skills[i];
                    break;
                }
            }

            // if they have the Scavenger skill, do a random chance thing and see if they should get 2 resources
            float chance = 0;
            int count = 1;

            if (scavengerSkill != null)
            {
                // player has this skill
                SkillLevel highestLevel = null;
                for (int i = 0; i < scavengerSkill.levels.Length; i++)
                {
                    if (scavengerSkill.levels[i].IsUnlocked)
                        highestLevel = scavengerSkill.levels[i];
                    else
                        break;
                }

                if (highestLevel != null)
                {
                    chance = highestLevel.Modifier;

                    // calculate the count?
                    //Random random = new Random();
                    float randomNumber = (float)Random.value;
                    Debug.Log("Random number: " + randomNumber + ", chance: " + chance);
                    if (randomNumber <= chance)
                    {
                        count = 2;
                    }
                }
            }

            Debug.Log("Add count: " + count);
            other.transform.parent.GetComponent<InventoryManager>().resourceCounts[(int)this.type] += count;
            this.gameObject.SetActive(false);
        }
    }
}
