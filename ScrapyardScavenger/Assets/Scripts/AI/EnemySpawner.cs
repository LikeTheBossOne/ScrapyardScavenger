using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public ChargerStats chargerPrefab;
    public ShamblerStats shamblerPrefab;
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
    // Start is called before the first frame update
    void Start()
    {
        spawnPoints = FindObjectsOfType<SpawnPoint>();
        chargerCount = 0;
        shamblerCount = 0;
        shamblerCoolDown = 0;
        chargerCoolDown = 0;
        shamblerInterval = 2;
        chargerInterval = 60;
        shamblerMax = 5;
        chargerMax = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (shamblerCoolDown <= 0)
        {
            if (shamblerCount < shamblerMax)
            {
                //spawn logic
                int selected = Random.Range(0, spawnPoints.Length);
                Instantiate(shamblerPrefab, spawnPoints[selected].location.position, spawnPoints[selected].location.rotation);
                shamblerCount++;
            }
            shamblerCoolDown = shamblerInterval;
        }
        else
        {
            shamblerCoolDown -= Time.deltaTime;
        }
    }

    public void onShamblerKill()
    {
        shamblerCount--;
    }
    public void onChargerKill()
    {
        chargerCount--;
    }
}
