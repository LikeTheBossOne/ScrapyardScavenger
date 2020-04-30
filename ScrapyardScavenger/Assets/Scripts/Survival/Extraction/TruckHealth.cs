using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TruckHealth : MonoBehaviourPunCallbacks
{
    public int maxHealth;
    public int currentHealth { get; private set; }

    public InGameDataManager dataManager;
    public InGamePlayerManager pManager;

    // Start is called before the first frame update
    void Start()
    {
       // if (photonView.IsMine)
            //dataManager = GetComponent<PlayerControllerLoader>().inGameDataManager;

        currentHealth = maxHealth;
        Debug.Log("Truck is starting with current health: " + currentHealth);

        // consider using this:
        pManager = FindObjectOfType<InGamePlayerManager>();
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
            
            foreach (GameObject player in pManager.players)
            {
                player.GetPhotonView().RPC("TruckTakeDamage", RpcTarget.All, (float) damage);
            }

            if (currentHealth <= 0)
            {
                foreach (GameObject player in pManager.players)
                {
                    player.GetPhotonView().RPC("PlayerDied", RpcTarget.All);
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
