using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
[RequireComponent(typeof(Collider))]
public class AcidSpit : MonoBehaviour
{
    public Collider Shooter { get; set; }
    public int maxExistTime = 5;
    public int velocity = 10;
    public Vector3 direction;


    public LayerMask hardLayers;

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            transform.position += direction * velocity * Time.deltaTime;
        }

    }
    private void OnEnable()
    {
        Destroy(gameObject, maxExistTime);
    }

    public void Shoot(Vector3 dir)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            direction = transform.forward.normalized;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject collObj = collision.gameObject;
            if (collObj.CompareTag("Player"))
            {
                //gameObject.GetPhotonView().RPC("CleanUpProjectiles", RpcTarget.All);
                collObj.GetPhotonView().RPC("TakeDamage", RpcTarget.All, Shooter.GetComponent<ShamblerAttacks>().spitDamage);
                PhotonNetwork.Destroy(gameObject);
            }
            else if (((1 << collObj.layer) & hardLayers.value) != 0)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
