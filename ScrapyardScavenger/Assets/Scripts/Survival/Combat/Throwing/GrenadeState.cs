using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UIElements;

public class GrenadeState : MonoBehaviour
{
    private bool thrown = false;
    public Grenade grenadeType;
    private InGameDataManager igdm;
    public Collider[] c;
    public GameObject player;

    private float aoe;
    private float detTime;
    private float damage;

    // Start is called before the first frame update
    void Start()
    {
        Transform view = GetComponent<Transform>();
        view.Translate(.2f,-.1f,.4f);
        aoe = grenadeType.areaOfEffect;
        damage = grenadeType.baseDamage;
        detTime = grenadeType.baseDetonationTime;
    //GetComponent<Rigidbody>().Sleep();
    //GetComponent<Rigidbody>().detectCollisions = false;
    //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
}

    // Update is called once per frame
    void Update()
    {
        if (!thrown)
        {
            //GetComponent<Rigidbody>().Sleep();
        }
        if (gameObject.active && Input.GetMouseButtonUp(0) && !thrown)
        {
            igdm = GetComponentInParent<Transform>().GetComponentInParent<PlayerControllerLoader>().inGameDataManager;
            Rigidbody rb = gameObject.AddComponent<Rigidbody>() as Rigidbody;
            thrown = true;
            //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            //GetComponent<Rigidbody>().detectCollisions = true;
            Transform angleLooking = GetComponentInParent<Transform>();
            GetComponent<Rigidbody>().WakeUp();
            GetComponent<Rigidbody>().AddForce(angleLooking.forward *500);
            GetComponent<Rigidbody>().useGravity = true;
            GetComponentInParent<Transform>().parent = null;
            igdm.currentWeapons[3] = null;
            igdm.grenadeThrown();
            StartCoroutine(Explosion());
        }
    }

    [PunRPC]
    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(detTime);
        Debug.Log("Boom!");
        c = Physics.OverlapSphere(GetComponent<Transform>().position, aoe);
        for(int i = 0; i < c.Length; i++)
        {
            if(c[i].gameObject.layer == 11)
            {
                if (c[i].gameObject.tag == "Shambler")
                {
                    c[i].gameObject.GetPhotonView().RPC("TakeDamageShambler", RpcTarget.All, (int)damage, 0);
                }
                else
                {
                    c[i].gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, (int)damage);
                }
            }
        }
    }
}
