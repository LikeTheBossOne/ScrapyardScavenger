using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Transform location;

    private void Start()
    {
        location = gameObject.transform;
    }
}
