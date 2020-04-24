using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerShoot : MonoBehaviourPunCallbacks
{
    private InGameDataManager inGameManager;

    public LayerMask enemyLayer;

    public GameObject bulletHolePrefab;

    public PlayerHUD pHud;

    public Transform gunParent;

    private float nextFireTime = 0;
    private bool wantsToShoot = false;
    private Coroutine reloadCoroutine;
    private Transform reloadingModel;
    private bool wantsToReload = false;

    void Start()
    {
        inGameManager = GetComponent<PlayerControllerLoader>().inGameDataManager;
        pHud = GetComponent<PlayerHUD>();

        inGameManager.OnEquipmentSwitched += EquipmentSwitched;
    }

    void Update()
    {
        if (!photonView.IsMine) return;


        Gun gun = inGameManager.getCurrentEquipment() as Gun;
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
			if ((Input.GetMouseButtonDown((int)MouseButton.LeftMouse)) || (Input.GetKeyDown("joystick button 5"))
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
			if ((Input.GetKeyDown(KeyCode.R) || (Input.GetKeyDown("joystick button 4")))
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
		this.GetComponent<PlayerHUD>().crossHairReloading();
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

        Gun gun = inGameManager.getCurrentEquipment() as Gun;

        if (gun.isShotgun)
        {
            gunParent.GetChild(inGameManager.currentWeaponIndex).GetComponent<GunState>().ammoCount++;
        }
        else
        {
            gunParent.GetChild(inGameManager.currentWeaponIndex).GetComponent<GunState>().ammoCount = gun.baseClipSize;
        }
        

        
        pHud.AmmoChanged(gunState.ammoCount, gun.baseClipSize);
        

        inGameManager.isReloading = false;
		this.GetComponent<PlayerHUD>().crossHairReloaded();
        gunState.reloadStop();
    }

    [PunRPC]
    void Shoot()
    {
        Gun gun = inGameManager.getCurrentEquipment() as Gun;
        if (gun == null)
        {
            Debug.Log("BAD");
            return;
        }

        GunState gunState = gunParent.GetChild(inGameManager.currentWeaponIndex).GetComponent<GunState>();
        gunState.bulletSound();
        Transform eyeCam = transform.Find("Cameras/Main Player Cam");
        

        if (gun.isShotgun)
        {

            for (int i = 0; i < gun.pelletCount; i++)
            {
                var rad = Random.Range(0, 360f) * Mathf.Deg2Rad;
                var spreadX = Random.Range(0.0f, 0.1f) * Mathf.Cos(rad);
                var spreadY = Random.Range(0.0f, 0.1f) * Mathf.Sin(rad);
                var deviation = new Vector3(spreadX, spreadY, 0);
                var dir = deviation + eyeCam.forward;

                Debug.DrawRay(eyeCam.position, dir * gun.range, Color.blue, 5);
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(eyeCam.position, dir, out hit, gun.range, enemyLayer))
                {
                    if (photonView.IsMine && hit.collider.gameObject.layer == 11)
                    {
                        GameObject enemy;
                        if (hit.collider.gameObject.transform.parent)
                        {
                            enemy = hit.collider.transform.root.gameObject;

                        }
                        else
                        {
                            enemy = hit.collider.gameObject;

                        }

                        this.GetComponent<PlayerHUD>().hitCrossHair();
                        if (enemy.tag == "Shambler")
                        {
                            enemy.GetPhotonView().RPC("TakeDamageShambler", RpcTarget.All, (int)gun.baseDamage, photonView.ViewID);
                        }
                        else
                        {
                            enemy.GetPhotonView().RPC("TakeDamage", RpcTarget.All, (int)gun.baseDamage);
                        }

                    }
                }
            }
        }
        else
        {
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(eyeCam.position, eyeCam.forward, out hit, 1000f, enemyLayer))
            {
                if (photonView.IsMine && hit.collider.gameObject.layer == 11)
                {
                    GameObject enemy;
                    if (hit.collider.gameObject.transform.parent)
                    {
                        enemy = hit.collider.transform.root.gameObject;

                    }
                    else
                    {
                        enemy = hit.collider.gameObject;

                    }

                    this.GetComponent<PlayerHUD>().hitCrossHair();
                    if (enemy.tag == "Shambler")
                    {
                        enemy.GetPhotonView().RPC("TakeDamageShambler", RpcTarget.All, (int)gun.baseDamage, photonView.ViewID);
                    }
                    else
                    {
                        enemy.GetPhotonView().RPC("TakeDamage", RpcTarget.All, (int)gun.baseDamage);
                    }

                }

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
    protected void TakeDamage(int damage)
    {
        GetComponent<Health>().TakeDamage(damage);
    }

    [PunRPC]
    public void KilledEnemy(int enemy)
    {
        if (photonView.IsMine)
        {
            if (enemy == (int)EnemyType.Shambler) 
                GetComponent<PlayerControllerLoader>().skillManager.GainXP((int)XPRewards.KillShambler);
            if (enemy == (int)EnemyType.Charger)
                GetComponent<PlayerControllerLoader>().skillManager.GainXP((int)XPRewards.KillCharger);
        }
    }

    void EquipmentSwitched()
    {
        if (reloadCoroutine != null)
            StopCoroutine(reloadCoroutine);

        if (reloadingModel != null)
            reloadingModel.localRotation = Quaternion.identity;
        
        inGameManager.isReloading = false;
		this.GetComponent<PlayerHUD>().crossHairReloaded();
        if (inGameManager.currentWeaponIndex != 3)
        {
            GunState gunState = gunParent.GetChild(inGameManager.currentWeaponIndex).GetComponent<GunState>();
            pHud.AmmoChanged(gunState.ammoCount, gunState.baseAmmo);
        }
        
    }
}
