using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AcidSpit : MonoBehaviour
{
    public GameObject origin { get; set; }
    public AIPlayerManager pManage { get; set; }
    public int maxExistTime = 10;
    private void OnEnable()
    {
        pManage = FindObjectOfType<AIPlayerManager>();
        Destroy(gameObject, maxExistTime);
    }

    public void Shoot(GameObject creator)
    {
        origin = creator;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.bounds.Intersects(origin.GetComponent<Collider>().bounds))
        {
            foreach (Transform player in pManage.players)
            {
                if (collision.collider.bounds.Contains(player.position))
                {
                    player.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, origin.GetComponent<ShamblerAttacks>().spitDamage);
                }
            }
            

            
            Destroy(gameObject);
        }
    }
}
