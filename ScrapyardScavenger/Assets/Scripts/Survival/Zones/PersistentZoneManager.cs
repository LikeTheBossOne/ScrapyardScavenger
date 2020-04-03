using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PersistentZoneManager : MonoBehaviourPun
{

    #region Singleton

    public static PersistentZoneManager Instance { get; private set; }

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

    public List<Zones> UnlockedZones;

    // Start is called before the first frame update
    void Start()
    {
        UnlockedZones = new List<Zones>();
        UnlockedZones.Add(Zones.Zone1); // Zone1 should start as active
    }

    [PunRPC]
    public void UnlockNewZone(int newZone)
    {
        UnlockedZones.Add((Zones)newZone);
    }

    /*[PunRPC]
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
    }*/
}
