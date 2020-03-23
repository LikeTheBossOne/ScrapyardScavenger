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
        gameObject.transform.position += direction * Velocity * Time.deltaTime;
     
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
        Debug.Log("Spit shot");
        //Debug.Log(creator);
        //shooter = creator;
        direction = transform.forward.normalized;
        //direction = dir.normalized;
        
    }
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected");
        //may need to change this over to rigidbody at some point
        Debug.Log("Collision with: " + collision.collider);
        Debug.Log("Owner hitbox: " + shooter);
        if (!shooter || !collision.collider.bounds.Intersects(shooter.bounds))
        {
            foreach (GameObject obj in pManage.players)
            {
                RectTransform player = obj.GetComponent<RectTransform>();

                if (collision.collider.bounds.Contains(player.position))
                {
                    Debug.Log("player hit");
                    player.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, shooter.GetComponent<ShamblerAttacks>().spitDamage, -1);
                }
            }
            

            
            //Destroy(gameObject);
        }
    }
}
