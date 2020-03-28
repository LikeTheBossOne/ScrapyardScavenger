using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PersistentBuyableManager : MonoBehaviourPun
{
    #region Singleton

    public static PersistentBuyableManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (this != Instance)
        {
            Destroy(gameObject);
        }
    }

    #endregion

    public List<bool> activeBuyables;

    public Transform buyables;

    void Start()
    {
        activeBuyables = new List<bool>();

        buyables = GameObject.FindWithTag("Buyables").transform;
        for (int i = 0; i < buyables.childCount; i++)
        {
            activeBuyables.Add(true);
        }
    }

    [PunRPC]
    public void RemoveBuyable(int childIndex)
    {
        activeBuyables[childIndex] = false;
        buyables.GetChild(childIndex).gameObject.SetActive(false);
    }

    [PunRPC]
    public void PlaceBuyables()
    {
        buyables = GameObject.FindWithTag("Buyables").transform;
        if (buyables == null)
            return;
        if (activeBuyables.Count == 0)
        {
            for (int i = 0; i < buyables.childCount; i++)
            {
                activeBuyables.Add(true);
            }
        }

        for (int i = 0; i < buyables.childCount; i++)
        {
            if (!activeBuyables[i])
            {
                buyables.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
