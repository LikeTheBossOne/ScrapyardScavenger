using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TruckHealth : MonoBehaviourPunCallbacks
{
    public int maxHealth = 500;
    public int currentHealth { get; private set; }

    public InGameDataManager dataManager;

    // Start is called before the first frame update
    void Start()
    {
       // if (photonView.IsMine)
            //dataManager = GetComponent<PlayerControllerLoader>().inGameDataManager;

        maxHealth = 500;
        currentHealth = maxHealth;
        Debug.Log("Truck is starting with current health: " + currentHealth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    void TakeDamage(int damage)
    {
        if (photonView.IsMine)
        {
            currentHealth -= damage;
            Debug.Log("Truck took " + damage + " damage! Truck has " + currentHealth + " left!!");
            
            GameObject[] playersArray = GameObject.FindGameObjectsWithTag("Player");
            Debug.Log("Player obj length: " + playersArray.Length);
            foreach (GameObject player in playersArray)
            {
                if (player.name == "Body")
                {
                    Debug.Log("Ignoring body");
                    continue;
                }
                else
                {
                    player.GetComponent<Health>().pHud.truckTakeDamage(damage);
                }
            }

            if (currentHealth <= 0)
            {
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                Debug.Log("Player obj length: " + players.Length);
                foreach (GameObject player in players)
                {
                    if (player.name == "Body")
                    {
                        Debug.Log("Ignoring body");
                        continue;
                    }
                    else
                    {
                        player.GetPhotonView().RPC("PlayerDied", RpcTarget.All);
                    }
                }
                /*
                Debug.Log("The truck ran out of health, causing both players to die!");
                //GameObject player1 = GameObject.Find("PhotonPlayer");
                player1.GetPhotonView().RPC("PlayerDied", RpcTarget.All);
                Debug.Log("Killed one player!");
                try
                {
                    GameObject player2 = GameObject.Find("PhotonPlayer");
                    player2.GetPhotonView().RPC("PlayerDied", RpcTarget.All);
                    Debug.Log("Killed a second player!");
                } catch (System.Exception e) {
                    Debug.Log("Second player does not exist, or was already dead!");
                }
                */
            }
        }

    }
}
