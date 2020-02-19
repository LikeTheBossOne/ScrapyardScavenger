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

    private float nextFireTime = 0;

    void Start()
    {
        equipmentManager = GetComponent<EquipmentManager>();
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        Gun gun = equipmentManager.getCurrentEquipment() as Gun;
        if (gun == null) return;


        // Semi-Auto
        if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse)
            && !gun.isAutomatic
            && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1 / ( gun.baseRateOfFire / 60);
            photonView.RPC("Shoot", RpcTarget.All);
        }

        // Auto
        if (Input.GetMouseButton((int)MouseButton.LeftMouse)
            && gun.isAutomatic
            && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1 / (gun.baseRateOfFire / 60);
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
