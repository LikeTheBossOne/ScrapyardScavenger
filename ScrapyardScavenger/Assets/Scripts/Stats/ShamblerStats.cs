using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShamblerStats : Stats, IPunObservable
{
    
    public float damage { get; private set; }
    
    // Start is called before the first frame update
    private void OnEnable()
    {
        health = 10;
        damage = 10;
        status = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (health <= 0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GameObject spawner = FindObjectOfType<EnemySpawner>().gameObject;
                spawner.GetPhotonView().RPC("onShamblerKill", RpcTarget.All);
            }
            Destroy(gameObject);

        }
    }

    [PunRPC]
    void TakeDamageShambler(int damage)
    {
            //, GameObject damager, int atkStatus
            //note GameObjects can be passed by RPC
            health = health - damage;
            /*if (atkStatus > 0)
            {
                status = atkStatus;
            }*/
            //GetComponentInParent<ShamblerDetection>().gotShot(damager);

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
