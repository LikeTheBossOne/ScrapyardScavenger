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
    public bool IsInitialized = false;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("PersistentZoneManger is starting");
        UnlockedZones = new List<Zones>();
        UnlockedZones.Add(Zones.Zone1); // Zone1 should start as active
        IsInitialized = true;
    }

    [PunRPC]
    public void UnlockNewZone(int newZone)
    {
        UnlockedZones.Add((Zones)newZone);
    }
}
