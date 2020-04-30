using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UIElements;

public class GrenadeState : MonoBehaviourPun
{
    private bool thrown = false;
    public Grenade grenadeType;
    private InGameDataManager igdm;
    public Collider[] c;
    private GameObject player;
    private Rigidbody r;

    public LayerMask layer;
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
        if (!photonView.IsMine) return;

        if (gameObject.activeSelf && Input.GetMouseButtonUp(0) && !thrown)
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
        if (thrown && grenadeType.name.Equals("Sticky Grenade") && collision.gameObject != player)
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
        c = Physics.OverlapSphere(GetComponent<Transform>().position, aoe, layer);
        HashSet<int> enemiesHit = new HashSet<int>();
        foreach (var hit in c)
        {
            if (hit.transform.root.tag == "Shambler"/* && c[i].GetType() == typeof(CharacterController)*/ )
            {
                if (enemiesHit.Add(hit.transform.root.gameObject.GetPhotonView().ViewID))
                {
                    hit.transform.root.gameObject.GetPhotonView().RPC("TakeDamageShambler", RpcTarget.All, (int)damage, player.GetComponent<PhotonView>().ViewID);
                }
            }
            else if (hit.transform.parent.gameObject.tag == "Player")
            {
                if (enemiesHit.Add(hit.transform.parent.gameObject.GetPhotonView().ViewID))
                {
                    hit.transform.parent.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, (int)damage);
                }
            }
        }
        PhotonNetwork.Destroy(this.gameObject);
    }
}
