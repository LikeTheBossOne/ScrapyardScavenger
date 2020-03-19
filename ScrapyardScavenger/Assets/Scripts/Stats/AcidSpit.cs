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
       
        //may need to change this over to rigidbody at some point
        //the trick for restoring projectiles is dealing with the collision issue
        //the same character on the other client technically has a different object
        if (!shooter || !collision.collider.bounds.Intersects(shooter.bounds))
        {

            if (PhotonNetwork.IsMasterClient)
            {
                foreach (GameObject obj in pManage.players)
                {

                    RectTransform player = obj.GetComponent<RectTransform>();

                    if (collision.collider.bounds.Contains(player.position))
                    {
                        if (collision.collider.bounds.Contains(player.position))
                        {

                            player.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, shooter.GetComponent<ShamblerAttacks>().spitDamage);
                        }
                    }
                }
                gameObject.GetPhotonView().RPC("CleanUpProjectiles", RpcTarget.All);
            }
            //idea, call rpc destroy on this object to destroy all

            //Destroy(gameObject);

        }
    }
    
    [PunRPC]
    public void CleanUpProjectiles()
    {
        Destroy(gameObject);
    }
}
