using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class NewXPCreator : MonoBehaviour
{
    public GameObject xpPrefab;

    private SkillManager skillManager;
    private Text totalXP;

    void Start()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameController");
        foreach (var obj in objs)
        {
            if (obj.GetPhotonView().IsMine)
            {
                skillManager = obj.GetComponent<SkillManager>();
                skillManager.OnXPChanged += OnXpChanged;

                break;
            }
        }

        totalXP = transform.GetChild(0).GetComponent<Text>();
        totalXP.text = $"{skillManager.GetTotalXP()}";
    }

    void Update()
    {
        
    }

    public void OnXpChanged(int amount)
    {
        GameObject newXP = Instantiate(xpPrefab, transform);
        if (amount < 0)
        {
            newXP.GetComponent<Text>().text = $"{amount}";
            newXP.GetComponent<Animator>().Play("Remove XP");
        }
        else
        {
            newXP.GetComponent<Text>().text = $"+{amount}";
            newXP.GetComponent<Animator>().Play("Add XP");
        }

        totalXP.text = $"{skillManager.GetTotalXP()}";
    }

    void OnDestroy()
    {
        skillManager.OnXPChanged -= OnXpChanged;
    }
}
