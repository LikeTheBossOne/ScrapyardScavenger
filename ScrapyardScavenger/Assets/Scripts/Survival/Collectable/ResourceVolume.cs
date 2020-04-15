using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class ResourceVolume : MonoBehaviourPun
{
    public LayerMask ground;

    public GameObject collectable;
    public int totalSpawnTries;

    [EnumFlags]
    [SerializeField]
    private ResourceTypeBitwise resourceFlags;
    private List<int> resources;

    private Bounds bounds;

    void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        SetResources();
        SpawnResources();
    }

    private void SetResources()
    {
        resources = new List<int>();
        for (int i = 1; i < (int) ResourceTypeBitwise.MAX; i <<= 1)
        {
            if (resourceFlags.HasFlag((ResourceTypeBitwise) i))
            {
                resources.Add((int)Math.Log(i, 2));
            }
        }

        if (resources.Count == 0)
        {
            Debug.LogError("Missing Resource in ResourceVolume on " + gameObject.name);
        }
    }

    private void SpawnResources()
    {
        float totalArea = 0;
        // Get total area so we can split spawncount equally
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            Vector3 size = child.GetComponent<MeshRenderer>().bounds.size;
            totalArea += size.x * size.z;
        }


        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            bounds = child.GetComponent<MeshRenderer>().bounds;
            float area = bounds.size.x * bounds.size.z;
            
            float binomialRandom = Random.Range(0, area) - Random.Range(0, area);
            int spawnTries = (int)Math.Round((area + binomialRandom) * totalSpawnTries / totalArea);
            int tryCount = 0;

            // Spawn the amount of resources
            while (tryCount < spawnTries)
            {
                float randX = Random.Range(bounds.min.x, bounds.max.x);
                float randZ = Random.Range(bounds.min.z, bounds.max.z);
                Vector3 randStartLoc = new Vector3(randX, 80, randZ);

                Debug.DrawRay(randStartLoc, Vector3.down * 100, Color.red, 10000f);

                RaycastHit hit;
                if (Physics.Raycast(randStartLoc, Vector3.down, out hit, 100f)
                    && (int)Mathf.Pow(2, hit.collider.gameObject.layer) == ground)
                {
                    // Set collectable
                    GameObject objToSpawn = PhotonNetwork.Instantiate(
                        Path.Combine("PhotonPrefabs", "First_aid_kit_1_Mat"),
                        hit.point + new Vector3(0, collectable.transform.localScale.y / 2, 0),
                        Quaternion.identity
                    );
                    objToSpawn.GetComponent<ResourcePickup>().type = (ResourceType)resources[Random.Range(0, resources.Count)];
                }

                tryCount++;
            }
        }
        
    }
}
