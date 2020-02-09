using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerShoot : MonoBehaviourPunCallbacks
{
    public EquipmentManager equipmentManager;
    public LayerMask enemyLayer;

    public GameObject bulletHolePrefab;

    void Start()
    {
        
    }

    void Update()
    {
        if (!photonView.IsMine) return;
        if (equipmentManager.getCurrentEquipment() as Gun == null) return;

        if (Input.GetMouseButton((int)MouseButton.LeftMouse))
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
        }
    }
}
