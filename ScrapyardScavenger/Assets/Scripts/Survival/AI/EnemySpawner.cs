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
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", shambName), spawnPoints[selected].location.position, spawnPoints[selected].location.rotation);
                shamblerCount++;
            }
            shamblerCoolDown = shamblerInterval;
        }
        else
        {
            shamblerCoolDown -= Time.deltaTime;
        }
    }
    [PunRPC]
    public void onShamblerKill()
    {
        shamblerCount--;
    }
    [PunRPC]
    public void onChargerKill()
    {
        chargerCount--;
    }
}
