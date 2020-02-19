using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ResourcePickup : MonoBehaviour
{
    public ResourceType type;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // If either player is within certain radius of this prefab, destroy this from map and add it to the inventory
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(this.type);
            other.transform.parent.GetComponent<InventoryManager>().resourceCounts[(int)this.type]++;
            this.gameObject.SetActive(false);
        }
    }
}
