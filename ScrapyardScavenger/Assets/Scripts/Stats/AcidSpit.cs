using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
[RequireComponent(typeof(Collider))]
public class AcidSpit : MonoBehaviour
{
    public GameObject shooter { get; set; }
    public AIPlayerManager pManage { get; set; }
    public int maxExistTime = 10;
    public int Velocity = 10;
    public Vector3 direction;

    private void Update()
    {
        gameObject.transform.position += direction * Velocity * Time.deltaTime;
    }
    private void OnEnable()
    {
        pManage = FindObjectOfType<AIPlayerManager>();
        Destroy(gameObject, maxExistTime);
    }
    [PunRPC]
    public void Shoot(GameObject creator, Vector3 dir)
    {
        shooter = creator;
        direction = dir.normalized;
        
    }
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected");
        //may need to change this over to rigidbody at some point
        Debug.Log("Collision with: " + collision.collider);
        Debug.Log("Owner hitbox: " + shooter);
        if (!collision.collider.bounds.Intersects(shooter.GetComponent<Collider>().bounds))
        {
            foreach (RectTransform player in pManage.players)
            {
                if (collision.collider.bounds.Contains(player.position))
                {
                    Debug.Log("player hit");
                    player.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, shooter.GetComponent<ShamblerAttacks>().spitDamage);
                }
            }
            

            
            //Destroy(gameObject);
        }
    }
}
