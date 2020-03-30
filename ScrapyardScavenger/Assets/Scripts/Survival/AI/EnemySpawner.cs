using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class EnemySpawner : MonoBehaviour
{
    public ChargerStats chargerPrefab;
    public ShamblerStats shamblerPrefab;
    public string shambName = "AITester";
    public SpawnPoint[] spawnPoints;
    private int chargerCount;
    private int shamblerCount;
    //how often units spawn in seconds
    public int shamblerInterval;
    public int chargerInterval;
    //cool downs are in seconds
    private float shamblerCoolDown;
    private float chargerCoolDown;
    public int shamblerMax;
    public int chargerMax;
    private const int startGracePeriod = 60;

    public int WaveNumber = 1; // consider changing to 0 and incorporating grace period
    private List<Zones> ActiveZones; // list of zones that the players are in


    // Start is called before the first frame update
    private void OnEnable()
    {
        spawnPoints = FindObjectsOfType<SpawnPoint>();
        chargerCount = 0;
        shamblerCount = 0;
        shamblerInterval = 2;
        chargerInterval = 60;
        //replace intervals with grace period to delay spawning cycle
        shamblerCoolDown = shamblerInterval;
        chargerCoolDown = chargerInterval;
        shamblerMax = 5;
        chargerMax = 2;
        ActiveZones = new List<Zones>();
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
                if (shamblerCount < shamblerMax)
                {
                    // spawn logic
                    int selected = Random.Range(0, spawnPoints.Length);

                    GameObject shambler = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", shambName), spawnPoints[selected].location.position, spawnPoints[selected].location.rotation);
                    // set the shambler's max health & damage based off of wave number
                    float waveModifier = 1.0f + (0.2f * (WaveNumber - 1));
                    shambler.GetComponent<Stats>().ModifyHealth(waveModifier);
                    shambler.GetComponent<ShamblerStats>().ModifyDamage(waveModifier);
                    Debug.Log("Spawned a Shambler with " + shambler.GetComponent<Stats>().GetCurrentHealth() + " health");
                    shamblerCount++;
                }
                shamblerCoolDown = shamblerInterval;
            }
            else
            {
                shamblerCoolDown -= Time.deltaTime;
            }

            // for testing purposes, make 'Z' advance the wave
            if (Input.GetKeyDown(KeyCode.Z))
            {
                NextWave();
            }
        }
        
    }

    public void NextWave()
    {
        WaveNumber++;
        Debug.Log("Wave number is now " + WaveNumber);
        // change the difficulty of any new enemies who spawn
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
