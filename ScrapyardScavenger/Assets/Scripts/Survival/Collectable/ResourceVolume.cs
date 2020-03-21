using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;

public class ResourceVolume : MonoBehaviourPun
{
    public Terrain playArea;
    public LayerMask ground;

    public GameObject spawnObj;
    public int spawnAmount;

    private Bounds bounds;

    void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        SpawnResources();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnResources()
    {
        Debug.Log("Spawning");
        int spawnedCount = 0;
        bounds = playArea.terrainData.bounds;

        // Spawn the amount of resources
        while (spawnedCount < spawnAmount)
        {
            float randX = Random.Range(playArea.transform.position.x + bounds.min.x, playArea.transform.position.x + bounds.max.x);
            float randZ = Random.Range(playArea.transform.position.z + bounds.min.z, playArea.transform.position.z + bounds.max.z);
            Vector3 randStartLoc = new Vector3(randX, 100, randZ);

            Debug.DrawRay(randStartLoc, Vector3.down * 100, Color.red, 10000f);

            RaycastHit hit;
            if (Physics.Raycast(randStartLoc, Vector3.down, out hit, 100f)
                && (int) Mathf.Pow(2, hit.collider.gameObject.layer) == ground)
            {
                // Randomize
                GameObject objToSpawn = PhotonNetwork.Instantiate(
                        Path.Combine("PhotonPrefabs", "Collectable"), 
                        hit.point + new Vector3(0, spawnObj.transform.localScale.y / 2, 0), 
                        Quaternion.identity
                    );
                objToSpawn.GetComponent<ResourcePickup>().type = (ResourceType) Random.Range(0, (int)ResourceType.SIZE);

                spawnedCount++;
            }
        }
    }
}
