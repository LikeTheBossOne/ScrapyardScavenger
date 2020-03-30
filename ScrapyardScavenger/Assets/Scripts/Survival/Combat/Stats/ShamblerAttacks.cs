﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;


public class ShamblerAttacks : MonoBehaviour
{
    public int meleeRange = 5;
    public int meleeRecharge = 2;
    public int meleeDamage = 5;
    public int spitRange = 10;
    public int spitRecharge = 10;
    public int spitDamage = 2;
    public float spitSize;
    public float meleeCoolDown { get; private set; }
    public float spitCoolDown { get; private set; }
    public AcidSpit projectile;
    public string projectileName = "AcidBall";
    // Start is called before the first frame update
    private void OnEnable()
    {
        meleeCoolDown = 0;
        spitCoolDown = 0;
        spitSize = projectile.GetComponent<SphereCollider>().radius;
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

    public void Spit(GameObject target)
    {
        
        if (spitCoolDown <= 0)
        {
            
            Vector3 toTarg = gameObject.transform.position - target.transform.position;
            if (toTarg.magnitude <= spitRange)
            {
                spitCoolDown = spitRecharge;
                Vector3 offset = new Vector3(spitSize + 0.1F,spitSize + 0.1F,spitSize + 0.1F);
                offset += GetComponent<Collider>().bounds.size;
                // offset.Scale(GetComponent<Collider>().bounds.size);
                offset.Scale(toTarg.normalized);
                GameObject shot = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", projectileName), gameObject.transform.position - offset, gameObject.transform.rotation);
                // shot.transform.LookAt(-toTarg);
                shot.GetComponent<AcidSpit>().shooter = gameObject.GetComponent<Collider>();
                Object[] args = { gameObject };
                // Debug.Log("RPC Call");
                shot.GetPhotonView().RPC("Shoot", RpcTarget.All, -toTarg);
                // shot.GetComponent<AcidSpit>().Shoot(gameObject, -toTarg);
                // insert play animation
                // target.GetPhotonView().RPC("TakeDamage", RpcTarget.All, spitDamage, gameObject, 1);
            }
            
        }
    } 

    public void Bite(GameObject target)
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
    public bool MeleeOnCoolDown()
    {
        return meleeCoolDown > 0;
    }
    public bool SpitOnCoolDown()
    {
        return spitCoolDown > 0;
    }
}
