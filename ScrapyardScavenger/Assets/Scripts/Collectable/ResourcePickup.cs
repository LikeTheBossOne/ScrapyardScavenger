using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePickup : MonoBehaviour
{
    public Transform[] players;

    // Start is called before the first frame update
    void Start()
    {
        players = new Transform[2];
    }

    // Update is called once per frame
    void Update()
    {
        // If either player is within certain radius of this prefab, destroy it from map and add it to the inventory

    }
}
