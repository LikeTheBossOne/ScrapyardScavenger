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
    private GameObject player;
    private Rigidbody r;

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
        player = GetComponentInParent<Transform>().GetComponentInParent<PlayerControllerLoader>().gameObject;
        PlayerHUD pHud = GetComponentInParent<Transform>().GetComponentInParent<PlayerHUD>();
        pHud.AmmoChanged(1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.active && Input.GetMouseButtonUp(0) && !thrown)
        {
            igdm = GetComponentInParent<Transform>().GetComponentInParent<PlayerControllerLoader>().inGameDataManager;
            Rigidbody rb = gameObject.AddComponent<Rigidbody>() as Rigidbody;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            thrown = true;
            Transform angleLooking = GetComponentInParent<Transform>();
            GetComponent<Rigidbody>().WakeUp();
            GetComponent<Rigidbody>().AddForce(angleLooking.forward *500);
            GetComponent<Rigidbody>().useGravity = true;
            GetComponentInParent<Transform>().parent = null;
            igdm.currentWeapons[3] = null;
            igdm.grenadeThrown();
            StartCoroutine(Explosion());
            r = rb;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (thrown && grenadeType.name.Equals("Sticky") && collision.gameObject != player)
        {
            r.constraints = RigidbodyConstraints.FreezeAll;
            GetComponentInParent<Transform>().parent = collision.gameObject.GetComponent<Transform>();
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
                if (c[i].gameObject.tag == "Shambler" && c[i].GetType() == typeof(CharacterController) )
                {
                    c[i].gameObject.GetPhotonView().RPC("TakeDamageShambler", RpcTarget.All, (int)damage, player.GetComponent<PhotonView>().ViewID);
                }
                else if (c[i].gameObject.tag == "Player")
                {
                    c[i].gameObject.GetComponent<Transform>().parent.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, (int)damage);
                }
            }
        }
        //Destroy(this.gameObject);
    }
}
