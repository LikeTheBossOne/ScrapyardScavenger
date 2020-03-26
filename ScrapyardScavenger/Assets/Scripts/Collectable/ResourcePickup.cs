using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


public class ResourcePickup : MonoBehaviour, IPunObservable
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
            int count = 1;
            // before adding to the count, check the player's Scavenger skill
            SkillLevel scavengerLevel = other.transform.parent.gameObject.GetComponent<PlayerControllerLoader>().skillManager.GetSkillByName("Scavenger");
            if (scavengerLevel != null)
            {
                // they have it, so calculate the count
                Debug.Log("Player has Scavenger, chance is: " + scavengerLevel.Modifier);

                // use random chance to see if the player should get 2 resources
                float chance = scavengerLevel.Modifier;
                float randomNumber = (float) Random.value;
                Debug.Log("randomNumber: " + randomNumber);
                if (randomNumber <= chance)
                {
                    Debug.Log("Collecting 2!");
                    count = 2;
                }
            }
            else Debug.Log("Player does NOT have Scavenger");


            if (other.transform.parent.gameObject.GetPhotonView().IsMine)
            {
                NotificationSystem.Instance.Notify(new Notification($"Picked up {count} {this.type.ToString()}", NotificationType.Neutral));
            }

            // Getting PlayerController's Inventory Manager
            for (int i = 0; i < count; i++)
                other.transform.parent.GetComponent<PlayerControllerLoader>().inGameDataManager.AddResourceToInventory(this.type);

            // Gain XP for collecting a resource
            int reward = (int) XPRewards.CollectResource;
            other.transform.parent.GetComponent<PlayerControllerLoader>().skillManager.GainXP(reward * count);
            Destroy(this.gameObject);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player; we are sending data to others
            stream.SendNext((int)type);
        }
        else
        {
            // Other player; receiving data
            this.type = (ResourceType)stream.ReceiveNext();
        }
    }
}
