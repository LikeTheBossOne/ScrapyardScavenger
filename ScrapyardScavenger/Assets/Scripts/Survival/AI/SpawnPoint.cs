using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Transform location;
    public bool IsFunctional = false;
    public Zones Zone;

    private void Start()
    {
        location = gameObject.transform;
        IsFunctional = false;
    }

    public float GetDistanceFromPlayer(GameObject player)
    {
        // get transform/position of player
        Transform playerTransform = player.gameObject.transform;

        // calculate distance from this spawnpoint to the player
        return Vector3.Distance(location.position, playerTransform.position);
    }
}
