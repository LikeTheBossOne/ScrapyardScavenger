using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerShoot : MonoBehaviourPunCallbacks
{
    private EquipmentManager equipmentManager;

    public LayerMask enemyLayer;

    public GameObject bulletHolePrefab;

    void Start()
    {
        equipmentManager = GetComponent<EquipmentManager>();
    }

    void Update()
    {
        if (!photonView.IsMine) return;
        if (equipmentManager.getCurrentEquipment() as Gun == null) return;

        if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse))
        {
            photonView.RPC("Shoot", RpcTarget.All);
        }
    }

    [PunRPC]
    void Shoot()
    {
        Transform eyeCam = transform.Find("Cameras/Main Player Cam");
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(eyeCam.position, eyeCam.forward, out hit, 1000f, enemyLayer))
        {
            GameObject newHole = Instantiate(bulletHolePrefab, hit.point + hit.normal * 0.001f, Quaternion.identity);
            newHole.transform.LookAt(hit.point + hit.normal);
            Destroy(newHole, 5f);

            if (photonView.IsMine && hit.collider.gameObject.layer == 11)
            {
                Gun gun = equipmentManager.getCurrentEquipment() as Gun;
                if (gun == null)
                {
                    Debug.Log("BAD");
                    return;
                }
                GameObject enemy = hit.collider.gameObject.transform.parent.gameObject;
                enemy.GetPhotonView().RPC("TakeDamage", RpcTarget.All, (int)gun.baseDamage);
            }
        }
    }

    [PunRPC]
    protected void TakeDamage(int damage)
    {
        GetComponent<Health>().TakeDamage(damage);
        Debug.Log("DAMAGE");
    }
}
