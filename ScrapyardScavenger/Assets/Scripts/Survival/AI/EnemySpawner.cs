using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class EnemySpawner : MonoBehaviourPun
{
    //public ChargerStats chargerPrefab;
    //public ShamblerStats shamblerPrefab;
    public string shambName;
    private List<SpawnPoint> AllSpawnPoints;
    private int chargerCount;
    private int shamblerCount;
    //how often units spawn in seconds
    public int shamblerInterval;
    public int chargerInterval;
    //cool downs are in seconds
    private float shamblerCoolDown;
    private float chargerCoolDown;
    public int startingShamblerMax;
    private int currentShamblerMax;
    public int chargerMax;
    private const int startGracePeriod = 60;

    public int WaveNumber = 1; // consider changing to 0 and incorporating grace period
    public int WaveInterval; // seconds between waves
    public float SkewedSpawnChance;
    public float CountModifier;
    public float DamageModifier;
    public float HealthModifier;
    private List<Zones> ActiveZones; // list of zones that the players are in
    private List<SpawnPoint> ActiveSpawnPoints; // list of spawn points that should be used based off of unlocked zones
    private Coroutine WaveCoroutine;
    private InGamePlayerManager playerManager;
    private bool initialSpawnPointLoad;
    public int absoluteShamblerCap;

    private bool SpawnEnabled;

    // Start is called before the first frame update
    private void OnEnable()
    {
        AllSpawnPoints = new List<SpawnPoint>();
        AllSpawnPoints.AddRange(FindObjectsOfType<SpawnPoint>());
        // consider going thru and initializing them to all be not functional

        ActiveSpawnPoints = new List<SpawnPoint>();
        chargerCount = 0;
        shamblerCount = 0;
        currentShamblerMax = startingShamblerMax;
        //replace intervals with grace period to delay spawning cycle
        shamblerCoolDown = shamblerInterval;
        chargerCoolDown = chargerInterval;
        ActiveZones = new List<Zones>();
        playerManager = GameObject.Find("PlayerList").GetComponent<InGamePlayerManager>();
        if (playerManager == null) Debug.Log("Null player list in EnemySpawner");
        initialSpawnPointLoad = false;
        SpawnEnabled = true;

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("I am the Master Client!");
            ActiveZones.Add(Zones.Zone1);
            WaveCoroutine = StartCoroutine(NextWave(WaveInterval));
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // if the zone manager hasn't started yet
            if (PersistentZoneManager.Instance != null
                && !PersistentZoneManager.Instance.IsInitialized)
            {
                return;
            }

            if (!initialSpawnPointLoad)
            {
                initialSpawnPointLoad = true;
                foreach (Zones zone in PersistentZoneManager.Instance.UnlockedZones)
                {
                    ActivateSpawnPointsForZone(zone);
                }
            }

            

            if (SpawnEnabled && shamblerCoolDown <= 0)
            {
                // there is room for more shamblers
                if (shamblerCount < currentShamblerMax)
                {
                    // spawn logic
                    List<SpawnPoint> pointsToSpawn = GetPossibleSpawnPoints();
                    int selected = Random.Range(0, pointsToSpawn.Count);

                    GameObject shambler = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", shambName), pointsToSpawn[selected].location.position, pointsToSpawn[selected].location.rotation);
                    // set the shambler's max health & damage based off of wave number
                    // maybe use RPC's to call these modify functions
                    float waveModifier = 0f;
                    if (WaveNumber == 1) waveModifier = 1.0f; // Call of Duty does: baseHealth ^ (1.1 * waveCount) << consider this in the future
                    else waveModifier = Mathf.Pow(HealthModifier, WaveNumber);//1.0f + (0.2f * (WaveNumber - 1));

                    // make this an RPC
                    shambler.GetPhotonView().RPC("ModifyHealth", RpcTarget.All, waveModifier);
                    shamblerCount++;
                }
                shamblerCoolDown = shamblerInterval;
            }
            else
            {
                shamblerCoolDown -= Time.deltaTime;
            }
        }
        
    }

    [PunRPC]
    public void ToggleSpawning()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnEnabled = !SpawnEnabled;
            if (SpawnEnabled) Debug.Log("Spawning is now enabled");
            else Debug.Log("Spawning is now disabled");
        }
    }

    private List<SpawnPoint> GetPossibleSpawnPoints()
    {
        if (PersistentZoneManager.Instance.UnlockedZones.Count == ActiveZones.Count)
        {
            return ActiveSpawnPoints;
        }
        List<SpawnPoint> pointsToSpawn = new List<SpawnPoint>();
        float randomNumber = Random.value;
        foreach (SpawnPoint point in ActiveSpawnPoints)
        {
            if (randomNumber <= SkewedSpawnChance && ActiveZones.Contains(point.Zone))
            {
                // then add only spawn points that are active zones
                pointsToSpawn.Add(point);
            }
            else if (randomNumber > SkewedSpawnChance && !ActiveZones.Contains(point.Zone))
            {
                // add if they are not in the active zone
                pointsToSpawn.Add(point);
            }
        }
        return pointsToSpawn;
    }

    private IEnumerator NextWave(int seconds)
    {
        while (true)
        {
            yield return new WaitForSeconds(seconds);
            WaveNumber++;
            // update the InGameUI somehow
            Debug.Log("Wave number is now " + WaveNumber);
            foreach (GameObject player in playerManager.players)
            {
                player.GetPhotonView().RPC("UpdateWaveInUI", RpcTarget.All, WaveNumber);
            }

            // change the amount of shamblers that will spawn
            currentShamblerMax = Mathf.Min((int) (CountModifier * currentShamblerMax), absoluteShamblerCap);
            Debug.Log("New current shambler max: " + currentShamblerMax);
        }
    }

    private void ActivateSpawnPointsForZone(Zones zone)
    {
        foreach (SpawnPoint point in AllSpawnPoints)
        {
            if (point.Zone == zone && !ActiveSpawnPoints.Contains(point))
            {
                // set this spawn point to active
                point.IsFunctional = true; // probably unnecessary
                ActiveSpawnPoints.Add(point);
                Debug.Log("Added a new active spawn point in zone " + (int)zone);
            }
        }
    }

    [PunRPC]
    public void UnlockZone(int thisZone)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Zones zoneEnum = (Zones)thisZone;
            if (!PersistentZoneManager.Instance.UnlockedZones.Contains(zoneEnum))
            {
                PersistentZoneManager.Instance.gameObject.GetPhotonView().RPC("UnlockNewZone", RpcTarget.All, thisZone);
                // for each spawn point
                ActivateSpawnPointsForZone(zoneEnum);
            }
            else
            {
                Debug.Log("Tried to unlock Zone " + thisZone + " but it has already been unlocked");
            }
            
        }

    }

    [PunRPC]
    public void UpdateActiveZones()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ActiveZones = new List<Zones>();
            foreach (GameObject player in playerManager.players)
            {
                if (!ActiveZones.Contains(player.GetComponent<ZoneManager>().GetCurrentZone()))
                    ActiveZones.Add(player.GetComponent<ZoneManager>().GetCurrentZone());
            }
        }
    }

    [PunRPC]
    public void onShamblerKill()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            shamblerCount--;
        }
        
    }
    [PunRPC]
    public void onChargerKill()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            chargerCount--;
        }
        
    }
}
