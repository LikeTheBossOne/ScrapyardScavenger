using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
[RequireComponent(typeof(Collider))]
public class AcidSpit : MonoBehaviour
{
    public Collider shooter { get; set; }
    public InGamePlayerManager pManage { get; set; }
    public int maxExistTime = 5;
    public int Velocity = 10;
    public Vector3 direction;

    public LayerMask playerLayer;
    public LayerMask groundLayer;
    public LayerMask mapLayer;

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            gameObject.transform.position += direction * Velocity * Time.deltaTime;
        }

    }
    private void OnEnable()
    {
        pManage = FindObjectOfType<InGamePlayerManager>();
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
        if (PhotonNetwork.IsMasterClient)
        {
            if (!shooter || !collision.collider.bounds.Intersects(shooter.bounds))
            {
                foreach (GameObject obj in pManage.players)
                {
                    RectTransform player = obj.GetComponent<RectTransform>();

                    if (collision.gameObject.CompareTag("Shambler"))
                    {
                        return;
                    }
                    if (((1 << collision.gameObject.layer) & playerLayer.value) != 0)
                    { 
                        player.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, shooter.GetComponent<ShamblerAttacks>().spitDamage);
                        PhotonNetwork.Destroy(gameObject);
                    }
                    else if (((1 << collision.gameObject.layer) & (mapLayer.value | groundLayer.value)) != 0)
                    {
                        PhotonNetwork.Destroy(gameObject);
                    }
                }
            }
        }
        //may need to change this over to rigidbody at some point
        //the trick for restoring projectiles is dealing with the collision issue
        //the same character on the other client technically has a different object
//        if (!shooter || !collision.collider.bounds.Intersects(shooter.bounds))
//        {
//
//            if (PhotonNetwork.IsMasterClient)
//            {
//                foreach (GameObject obj in pManage.players)
//                {
//
//                    RectTransform player = obj.GetComponent<RectTransform>();
//
//                    if (collision.collider.bounds.Contains(player.position))
//                    {
//                        if (collision.collider.bounds.Contains(player.position))
//                        {
//
//                            player.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, shooter.GetComponent<ShamblerAttacks>().spitDamage);
//                        }
//                    }
//                }
//                PhotonNetwork.Destroy(gameObject);
//            }
//        }
    }
}
