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

    public LayerMask groundLayer;
    public LayerMask mapLayer;

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            transform.position += direction * Velocity * Time.deltaTime;
        }

    }
    private void OnEnable()
    {
        pManage = FindObjectOfType<InGamePlayerManager>();
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
                collObj.GetPhotonView().RPC("TakeDamage", RpcTarget.All, shooter.GetComponent<ShamblerAttacks>().spitDamage);
                PhotonNetwork.Destroy(gameObject);
            }
            else if (((1 << collObj.layer) & (mapLayer.value | groundLayer.value)) != 0)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
