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
    private List<Zones> ActiveZones; // list of zones that the players are in
    private List<Zones> UnlockedZones;
    private List<SpawnPoint> ActiveSpawnPoints; // list of spawn points that should be used based off of unlocked zones
    private Coroutine WaveCoroutine;

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
        UnlockedZones = new List<Zones>();

        if (PhotonNetwork.IsMasterClient)
        {
            UnlockedZones.Add(Zones.Zone1); // change this to an RPC?
            ActivateSpawnPointsForZone(Zones.Zone1);

            WaveCoroutine = StartCoroutine(NextWave(WaveInterval));

            // actually, check to see if the player has unlocked any of the other zones
            // and add them appropriately, for persistence
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // calculate which zones the players are in
            // do this later

            if (shamblerCoolDown <= 0)
            {
                // there is room for more shamblers
                if (shamblerCount < currentShamblerMax)
                {
                    // spawn logic
                    int selected = Random.Range(0, ActiveSpawnPoints.Count);

                    GameObject shambler = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", shambName), ActiveSpawnPoints[selected].location.position, ActiveSpawnPoints[selected].location.rotation);
                    // set the shambler's max health & damage based off of wave number
                    // maybe use RPC's to call these modify functions
                    float waveModifier = 1.0f + (0.2f * (WaveNumber - 1));
                    shambler.GetComponent<Stats>().ModifyHealth(waveModifier);
                    shambler.GetComponent<ShamblerStats>().ModifyDamage(waveModifier);
                    Debug.Log("Spawned a Shambler in Zone " + ActiveSpawnPoints[selected].Zone);
                    shamblerCount++;
                    Debug.Log("There are now " + shamblerCount + " shamblers");
                }
                //else Debug.Log("Reached Shambler limit");
                shamblerCoolDown = shamblerInterval;
            }
            else
            {
                shamblerCoolDown -= Time.deltaTime;
            }
        }
        
    }

    private IEnumerator NextWave(int seconds)
    {
        while (true)
        {
            yield return new WaitForSeconds(seconds);
            WaveNumber++;
            Debug.Log("Wave number is now " + WaveNumber);

            // change the amount of shamblers that will spawn
            currentShamblerMax *= (int)(1.0f + (0.4f * (WaveNumber - 1)));
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
            if (!UnlockedZones.Contains(zoneEnum))
            {
                UnlockedZones.Add(zoneEnum);
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
