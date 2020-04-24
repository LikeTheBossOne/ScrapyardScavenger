using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AnimationUtilities : MonoBehaviour
{
    private Animator animator;
    public Transform Guns;
    public Transform Grenades;
    private InGameDataManager manager;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        manager = GetComponentInParent<PlayerControllerLoader>().inGameDataManager;
        manager.OnEquipmentSwitched += EquipmentSwitch;
    }

    private void OnDestroy()
    {
        manager.OnEquipmentSwitched -= EquipmentSwitch;
    }
    // Update is called once per frame
    //void Update()
    //{

    //}

    public void EndJump()
    {
        if (animator)
        {
            animator.SetBool("Jump", false);
            Debug.Log("End Jump.");
        }
        else
        {
            Debug.Log("Animator: " + animator);
        }
        
    }

    [PunRPC]
    public void EquipWeapon(int index)
    {
        Debug.Log("AnimUtility Equip RPC");
        if (index == 0 || index == 1)
        {
            if (Guns.GetChild(index).tag.Equals("Pistol"))
            {
                animator.SetBool("Pistol", true);
            }
            else
            {
                animator.SetBool("Pistol", false);
            }
            
        }
    }

    void EquipmentSwitch()
    {
        Debug.Log("AnimUtil equip");
        foreach (Transform gun in Guns)
        {
            if (gun.gameObject.activeSelf)
            {
                if (gun.tag.Equals("Pistol"))
                {
                    animator.SetBool("Pistol", true);
                    
                }
                else
                {
                    animator.SetBool("Pistol", false);
                }
                animator.SetBool("Grenade", false);
            }
        }
        foreach (Transform boom in Grenades)
        {
            if (boom.gameObject.activeSelf)
            {
                animator.SetBool("Grenade", true);
            }
        }
    }


    public void EndHeal()
    {
        Debug.Log("Med animation");
        GetComponent<GunIkController>().IkActive = true;
        transform.GetChild(1).gameObject.SetActive(true);
        animator.SetBool("Medkit", false);
    }
}
