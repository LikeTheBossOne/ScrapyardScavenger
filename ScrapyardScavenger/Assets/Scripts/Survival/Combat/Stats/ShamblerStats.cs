using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShamblerStats : Stats, IPunObservable
{

    public float damage; //{ get; private set; }
    
    // Start is called before the first frame update
    private void OnEnable()
    {
        health = baseHealth;
        //damage = 10;
        status = 0;
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    public void ModifyDamage(float modifier)
    {
        damage = damage * modifier;
    }

    [PunRPC]
    void TakeDamageShambler(int damage, int shooterID)
    {
        //, GameObject damager, int atkStatus
        //note GameObjects can be passed by RPC
        health -= damage;



        if (health <= 0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GameObject spawner = FindObjectOfType<EnemySpawner>().gameObject;
                spawner.GetPhotonView().RPC("onShamblerKill", RpcTarget.All);
                // notify the player so he can change his XP
                // find the player first
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject player in players)
                {
                    if (player.name == "Body")
                    {
                        continue;
                    }
                    if (player.GetPhotonView().ViewID == shooterID)
                    {
                        player.GetPhotonView().RPC("KilledEnemy", RpcTarget.All, (int)EnemyType.Shambler);
                    }

                }
            }


            Destroy(gameObject);

        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
        }
        else
        {
            this.health = (int)stream.ReceiveNext();
        }
    }
}
