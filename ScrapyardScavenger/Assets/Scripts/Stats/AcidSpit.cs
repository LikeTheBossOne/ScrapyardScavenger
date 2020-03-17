using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
[RequireComponent(typeof(Collider))]
public class AcidSpit : MonoBehaviour
{
    public Collider shooter { get; set; }
    public AIPlayerManager pManage { get; set; }
    public int maxExistTime = 10;
    public int Velocity = 10;
    public Vector3 direction;

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            gameObject.transform.position += direction * Velocity * Time.deltaTime;
        }

    }
    private void OnEnable()
    {
        pManage = FindObjectOfType<AIPlayerManager>();
        Destroy(gameObject, maxExistTime);
    }
    [PunRPC]
    public void Shoot( Vector3 dir)
    {
        //, Vector3 dir
        //shooter = creator;
        direction = transform.forward.normalized;
        //direction = dir.normalized;
        
    }
    void OnCollisionEnter(Collision collision)
    {
       
        //may need to change this over to rigidbody at some point
        if (!shooter || !collision.collider.bounds.Intersects(shooter.bounds))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                foreach (RectTransform player in pManage.players)
                {
                    if (collision.collider.bounds.Contains(player.position))
                    {

                        player.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, shooter.GetComponent<ShamblerAttacks>().spitDamage);
                    }
                }
            }
            Destroy(gameObject);
        }
    }
}
