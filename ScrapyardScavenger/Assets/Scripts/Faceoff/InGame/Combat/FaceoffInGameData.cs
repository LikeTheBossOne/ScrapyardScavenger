using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class FaceoffInGameData : MonoBehaviourPun
{
    public Transform gunParent;
    public Weapon[] currentWeapons = new Weapon[2];
    public bool isReloading = false;

    public delegate void EquipmentSwitched();
    public event EquipmentSwitched OnEquipmentSwitched;
    public int currentWeaponIndex;
    private GameObject currentObject;

    private GameObject ui;

    void Start()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("EquipWeapon", RpcTarget.All, 0);

            ui = GameObject.Find("In-Game UI");
            DisplayWeaponInfo weapon1 = ui.transform.Find("WeaponSlot1").GetComponent<DisplayWeaponInfo>();
            weapon1.faceoffData = this;
            weapon1.weapon = (Weapon) GetEquipment(0);

            DisplayWeaponInfo weapon2 = ui.transform.Find("WeaponSlot2").GetComponent<DisplayWeaponInfo>();
            weapon2.faceoffData = this;
            weapon2.weapon = (Weapon)GetEquipment(1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)
            && currentWeaponIndex != 0
            && currentWeapons[0] != null)
            photonView.RPC("EquipWeapon", RpcTarget.All, 0);
        if (Input.GetKeyDown(KeyCode.Alpha2)
            && currentWeaponIndex != 1
            && currentWeapons[1] != null)
            photonView.RPC("EquipWeapon", RpcTarget.All, 1);
    }

    [PunRPC]
    void EquipWeapon(int index)
    {
        if (currentObject != null)
            currentObject.SetActive(false);


        Transform parent;
        if (index == 0) parent = gunParent;
        else if (index == 1) parent = gunParent;
        else return;

        currentObject = parent.Equals(gunParent) ? parent.GetChild(index).gameObject : parent.GetChild(0).gameObject;

        currentObject.SetActive(true);
        currentWeaponIndex = index;

        OnEquipmentSwitched?.Invoke();
    }
    public Equipment GetCurrentEquipment()
    {
        if (currentWeaponIndex == -1) return null;
        return currentWeapons[currentWeaponIndex];
    }

    public Equipment GetEquipment(int index)
    {
        return currentWeapons[index];
    }
}
