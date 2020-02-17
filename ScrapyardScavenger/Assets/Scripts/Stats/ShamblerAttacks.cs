using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShamblerAttacks : MonoBehaviour
{
    public int meleeRange;
    public int meleeRecharge;
    public int meleeDamage;
    public int spitRange;
    public int spitRecharge;
    public int spitDamage;
    private float coolDown;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (coolDown > 0)
        {
            coolDown -= Time.deltaTime;
        }
    }

    public void spit(GameObject target)
    {
        //move this to AI just before call
        //gameObject.transform.LookAt(target, );
        if (coolDown <= 0)
        {
            
            Vector3 toTarg = gameObject.transform.position - target.transform.position;
            if (toTarg.magnitude <= spitRange)
            {
                coolDown = spitRecharge;
                //insert play animation
                target.GetPhotonView().RPC("TakeDamage", RpcTarget.All, [spitDamage, gameObject, 1]);
            }
            
        }
    } 

    public void bite(GameObject target)
    {
        if (coolDown <= 0)
        {
            Vector3 toTarg = gameObject.transform.position - target.transform.position;
            if (toTarg.magnitude <= meleeRange)
            {
                coolDown = meleeRecharge;
                //insert play animation
                target.GetPhotonView().RPC("TakeDamage", RpcTarget.All, [meleeDamage, gameObject, 0]);
            }
        }
    }
}
