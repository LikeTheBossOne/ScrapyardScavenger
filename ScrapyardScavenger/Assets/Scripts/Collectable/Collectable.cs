using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Collectable<T>
{
    public GameObject prefab;

    void OnTriggerEnter(PhotonPlayer p)
    {
        Destroy(this);
    }
}
