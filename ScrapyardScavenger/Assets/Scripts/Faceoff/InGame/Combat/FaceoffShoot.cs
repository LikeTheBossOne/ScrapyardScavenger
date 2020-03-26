using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UIElements;

public class FaceoffShoot : MonoBehaviourPun
{
    public FaceoffInGameData inGameManager;

    public LayerMask enemyLayer;

    public GameObject bulletHolePrefab;

    public FaceoffPlayerHUD pHud;

    public Transform gunParent;

    private float nextFireTime = 0;
    private bool wantsToShoot = false;
    private Coroutine reloadCoroutine;
    private Transform reloadingModel;
    private bool wantsToReload = false;

    void Start()
    {
        pHud = GetComponent<FaceoffPlayerHUD>();

        inGameManager.OnEquipmentSwitched += EquipmentSwitched;
    }

    void Update()
    {
        if (!photonView.IsMine) return;


        Gun gun = inGameManager.GetCurrentEquipment() as Gun;
        if (gun == null) return;
        GunState gunState = gunParent.GetChild(inGameManager.currentWeaponIndex).GetComponent<GunState>();

        // No Ammo
        if (gunState != null
            && !inGameManager.isReloading
            && gunState.ammoCount <= 0)
        {
            reloadCoroutine = StartCoroutine(Reload(gun.reloadTime));
        }

        pHud.AmmoChanged(gunState.ammoCount, gunState.baseAmmo);


        if (!inGameManager.isReloading
            && gunState.ammoCount > 0)
        {
            // Semi-Auto
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

            // Reload
            if (Input.GetKeyDown(KeyCode.R)
                && gunState.ammoCount < gunState.baseAmmo)
            {
                reloadCoroutine = StartCoroutine(Reload(gun.reloadTime));
            }
        }
    }

    void FixedUpdate()
    {
    }

    void OnDestroy()
    {
        inGameManager.OnEquipmentSwitched -= EquipmentSwitched;
    }

    IEnumerator Reload(float wait)
    {
        inGameManager.isReloading = true;
        this.GetComponent<FaceoffPlayerHUD>().crossHairReloading();
        reloadingModel = gunParent.GetChild(inGameManager.currentWeaponIndex).GetChild(0);

        // ANIMATION
        var animator = gunParent.GetChild(inGameManager.currentWeaponIndex).GetComponent<Animator>();
        GunState gunState = gunParent.GetChild(inGameManager.currentWeaponIndex).GetComponent<GunState>();
        if (animator != null)
        {
            animator.speed = 1.0f / wait;
            animator.Play("gun_reload", 0, 0);
            gunState.reloadSound();
        }

        yield return new WaitForSeconds(wait);

        Gun gun = inGameManager.GetCurrentEquipment() as Gun;
        gunParent.GetChild(inGameManager.currentWeaponIndex).GetComponent<GunState>().ammoCount = gun.baseClipSize;
        pHud.AmmoChanged(gun.baseClipSize, gun.baseClipSize);

        inGameManager.isReloading = false;
        this.GetComponent<FaceoffPlayerHUD>().crossHairReloaded();
        gunState.reloadStop();
    }

    [PunRPC]
    void Shoot()
    {
        GunState gunState = gunParent.GetChild(inGameManager.currentWeaponIndex).GetComponent<GunState>();
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
                Gun gun = inGameManager.GetCurrentEquipment() as Gun;
                if (gun == null)
                {
                    Debug.Log("BAD");
                    return;
                }
                GameObject enemy;
                if (hit.collider.gameObject.transform.parent)
                {
                    enemy = hit.collider.gameObject.transform.parent.gameObject;

                }
                else
                {
                    enemy = hit.collider.gameObject;

                }

                this.GetComponent<FaceoffPlayerHUD>().hitCrossHair();
                enemy.GetPhotonView().RPC("TakeDamage", RpcTarget.All, (int)gun.baseDamage, PhotonNetwork.LocalPlayer.ActorNumber);

            }

        }

        if (photonView.IsMine)
        {
            gunState = gunParent.GetChild(inGameManager.currentWeaponIndex).GetComponent<GunState>();
            gunState.ammoCount--;
            pHud.AmmoChanged(gunState.ammoCount, gunState.baseAmmo);
        }
    }

    [PunRPC]
    protected void TakeDamage(int damage, int actor)
    {
        GetComponent<FaceoffHealth>().TakeDamage(damage, actor);
    }

    void EquipmentSwitched()
    {
        if (reloadCoroutine != null)
            StopCoroutine(reloadCoroutine);

        if (reloadingModel != null)
            reloadingModel.localRotation = Quaternion.identity;

        inGameManager.isReloading = false;
        this.GetComponent<FaceoffPlayerHUD>().crossHairReloaded();
        GunState gunState = gunParent.GetChild(inGameManager.currentWeaponIndex).GetComponent<GunState>();
        pHud.AmmoChanged(gunState.ammoCount, gunState.baseAmmo);
    }
}
