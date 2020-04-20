using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ZombieAnimUtilities : MonoBehaviour
{
    private ShamblerAI brain;
    // Start is called before the first frame update
    void Start()
    {
        brain = GetComponentInParent<ShamblerAI>();
    }

    // Update is called once per frame
   // void Update()
   // {
        
   // }

    public void Die()
    {
        gameObject.GetComponentInParent<PhotonView>().RPC("CleanUp", RpcTarget.All);
    }

    public void EndAttack()
    {
        gameObject.GetComponentInParent<PhotonView>().RPC("EndSpit", RpcTarget.All);
    }

    public void Shoot()
    {
        
        if (brain.currentState == ShamblerAI.State.spit)
        {
            brain.weapons.Spit(brain.senses.detected.gameObject);
        }
    }
}
