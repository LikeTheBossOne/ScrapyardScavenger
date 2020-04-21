using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;


public class ShamblerAttacks : MonoBehaviour
{
    public float meleeRange;// = 5;
    public int meleeRecharge;// = 2;
    public int meleeDamage;// = 5;
    public float spitRange;// = 10;
    public int spitRecharge;// = 10;
    public int spitDamage;
    private float spitSize;
    public float meleeCoolDown { get; private set; }
    public float spitCoolDown { get; private set; }
    public AcidSpit projectile;
    public string projectileName = "Magic fire 0";

    public Transform mouth;
    public Transform head;
    // Start is called before the first frame update
    private void OnEnable()
    {
        meleeCoolDown = 0;
        spitCoolDown = 0;
        spitSize = projectile.GetComponent<SphereCollider>().radius;
        //spitDamage = (int) GetComponent<ShamblerStats>().damage;
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
        if (PhotonNetwork.IsMasterClient)
        {
            GetComponent<ShamblerAI>().SetDestination(gameObject.transform.position);

            Vector3 toTarg = target.transform.position - head.position;
            spitCoolDown = spitRecharge;

            GameObject shot = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", projectileName), head.position, gameObject.transform.rotation);
            AcidSpit spit = shot.GetComponent<AcidSpit>();
            spit.Shooter = gameObject.GetComponent<Collider>();            
        }
    } 

    public void Bite(GameObject target)
    {
        meleeCoolDown = meleeRecharge;
        target.GetPhotonView().RPC("TakeDamage", RpcTarget.All, meleeDamage);
        // Insert Animation
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
