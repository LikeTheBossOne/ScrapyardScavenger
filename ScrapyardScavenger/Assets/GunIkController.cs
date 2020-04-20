using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GunIkController : MonoBehaviour
{

    public bool IkActive;
    private Transform LeftTarget;
    private Transform RightTarget;
    public Transform Guns;
    private Transform current;
    protected Animator animator;
    private int slot;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        slot = 0;
        if (Guns.GetChild(slot))
        {
            current = Guns.GetChild(slot);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (animator)
        {
            if (IkActive)
            {

            }
        }
    }

    [PunRPC]
    void EquipWeapon(int index)
    {
        if (index == 0 || index == 1)
        {
            slot = index;
            if (Guns.GetChild(slot))
            {
                current = Guns.GetChild(slot);
            }
        }
    }
}
