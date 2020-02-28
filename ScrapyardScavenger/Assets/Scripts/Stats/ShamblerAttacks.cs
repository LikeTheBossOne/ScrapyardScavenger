using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class ShamblerAttacks : MonoBehaviour
{
    public int meleeRange = 5;
    public int meleeRecharge = 2;
    public int meleeDamage = 5;
    public int spitRange = 10;
    public int spitRecharge = 10;
    public int spitDamage = 2;
    public float meleeCoolDown { get; private set; }
    public float spitCoolDown { get; private set; }
    public AcidSpit projectile;
    // Start is called before the first frame update
    void Start()
    {
        meleeCoolDown = 0;
        spitCoolDown = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (meleeCoolDown > 0)
        {
            meleeCoolDown -= Time.deltaTime;
        }
        if (spitCoolDown > 0)
        {
            spitCoolDown -= Time.deltaTime;
        }
    }

    public void spit(GameObject target)
    {
        
        if (spitCoolDown <= 0)
        {
            
            Vector3 toTarg = gameObject.transform.position - target.transform.position;
            if (toTarg.magnitude <= spitRange)
            {
                spitCoolDown = spitRecharge;
                AcidSpit shot = Instantiate(projectile, gameObject.transform.position, gameObject.transform.rotation);
                shot.Shoot(gameObject, -toTarg);
                //insert play animation
                //target.GetPhotonView().RPC("TakeDamage", RpcTarget.All, spitDamage, gameObject, 1);
            }
            
        }
    } 

    public void bite(GameObject target)
    {
        if (meleeCoolDown <= 0)
        {
            Vector3 toTarg = gameObject.transform.position - target.transform.position;
            if (toTarg.magnitude <= meleeRange)
            {
                meleeCoolDown = meleeRecharge;
                //insert play animation
                //target.GetPhotonView().RPC("TakeDamage", RpcTarget.All,  meleeDamage, gameObject, 0);
            }
        }
    }
    public bool meleeOnCoolDown()
    {
        return meleeCoolDown > 0;
    }
    public bool spitOnCoolDown()
    {
        return spitCoolDown > 0;
    }
}
