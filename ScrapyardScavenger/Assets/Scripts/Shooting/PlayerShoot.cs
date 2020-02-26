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

    public PlayerHUD pHud;

    public Transform gunParent;

    private float nextFireTime = 0;
    private Coroutine reloadCoroutine;

    void Start()
    {
        equipmentManager = GetComponent<EquipmentManager>();
        pHud = GetComponent<PlayerHUD>();

        equipmentManager.OnEquipmentSwitched += EquipmentSwitched;
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        Gun gun = equipmentManager.getCurrentEquipment() as Gun;
        if (gun == null) return;
        GunState gunState = gunParent.GetChild(equipmentManager.currentIndex).GetComponent<GunState>();

        // No Ammo
        if (gunState != null
            && !equipmentManager.isReloading
            && gunState.ammoCount <= 0)
        {
            reloadCoroutine = StartCoroutine(Reload(gun.reloadTime));
        }


        // Semi-Auto
        if (!equipmentManager.isReloading
            && gunState.ammoCount > 0)
        {
            if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse)
                && !gun.isAutomatic
                && Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + 1 / (gun.baseRateOfFire / 60);
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

            if (Input.GetKeyDown(KeyCode.R)
                && gunState.ammoCount < gunState.baseAmmo)
            {
                reloadCoroutine = StartCoroutine(Reload(gun.reloadTime));
            }
        }
        
    }

    IEnumerator Reload(float wait)
    {
        equipmentManager.isReloading = true;

        // ANIMATION
        var animator = gunParent.GetChild(equipmentManager.currentIndex).GetComponent<Animator>();
        GunState gunState = gunParent.GetChild(equipmentManager.currentIndex).GetComponent<GunState>();
        if (animator != null)
        {
            animator.speed = 1.0f / wait;
            animator.Play("gun_reload", 0, 0);
            gunState.reloadSound();
        }

        yield return new WaitForSeconds(wait);

        Gun gun = equipmentManager.getCurrentEquipment() as Gun;
        gunParent.GetChild(equipmentManager.currentIndex).GetComponent<GunState>().ammoCount = gun.baseClipSize;
        pHud.AmmoChanged(gun.baseClipSize, gun.baseClipSize);

        equipmentManager.isReloading = false;
        gunState.reloadStop();
    }

    [PunRPC]
    void Shoot()
    {
        GunState gunState = gunParent.GetChild(equipmentManager.currentIndex).GetComponent<GunState>();
        gunState.bulletSound();
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

            // Ammo
            if (photonView.IsMine)
            {
                gunState = gunParent.GetChild(equipmentManager.currentIndex).GetComponent<GunState>();
                gunState.ammoCount--;
                pHud.AmmoChanged(gunState.ammoCount, gunState.baseAmmo);
            }
        }
    }

    [PunRPC]
    protected void TakeDamage(int damage)
    {
        GetComponent<Health>().TakeDamage(damage);
        Debug.Log("DAMAGE");
    }

    void EquipmentSwitched()
    {
        if (reloadCoroutine != null) StopCoroutine(reloadCoroutine);
        equipmentManager.isReloading = false;

        GunState gunState = gunParent.GetChild(equipmentManager.currentIndex).GetComponent<GunState>();
        pHud.AmmoChanged(gunState.ammoCount, gunState.baseAmmo);
    }
}
