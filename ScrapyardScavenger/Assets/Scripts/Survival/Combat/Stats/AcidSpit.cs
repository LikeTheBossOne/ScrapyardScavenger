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
    public int velocity = 15;

    public LayerMask hardLayers;


    private void FixedUpdate()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            transform.position += gameObject.transform.forward * velocity * Time.deltaTime; // instead of direction
        }

    }
    private void OnEnable()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        Destroy(gameObject, maxExistTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SpitCollide(collision);
        }
    }

    private void SpitCollide(Collision collision)
    {
        GameObject collObj = collision.gameObject;
        if (collObj.CompareTag("Player"))
        {
            collObj.GetPhotonView().RPC("TakeDamage", RpcTarget.All, Shooter.GetComponent<ShamblerAttacks>().spitDamage);
            PhotonNetwork.Destroy(gameObject);
        }
        else if (collObj.CompareTag("Truck"))
        {
            collObj.GetPhotonView().RPC("TakeDamage", RpcTarget.All, Shooter.GetComponent<ShamblerAttacks>().spitDamage);
            PhotonNetwork.Destroy(gameObject);
        }
        else if (((1 << collObj.layer) & hardLayers.value) != 0)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
