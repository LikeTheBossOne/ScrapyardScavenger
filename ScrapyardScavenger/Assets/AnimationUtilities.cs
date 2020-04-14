using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AnimationUtilities : MonoBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
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
        if (index >= 0 && index < 5)
        {
            animator.SetInteger("EquipEnum", index);
        }
    }
}
