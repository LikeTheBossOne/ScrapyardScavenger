using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillClickListener : MonoBehaviour
{
    public int skillIndex;
    public int levelIndex;
    public GameObject observer;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(ClickSkill);
    }

    void ClickSkill()
    {
        observer.GetComponent<SkillTreeObserver>().ClickSkill(skillIndex, levelIndex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
