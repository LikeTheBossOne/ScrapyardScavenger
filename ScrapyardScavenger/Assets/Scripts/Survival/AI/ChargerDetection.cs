using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChargerDetection : MonoBehaviour
{
    public float timeShotAt;
    //in unity distance units
    public float visionLimit = 20.0F;
    public Transform detected;
    public Transform distToDetected;
    public Transform vendeta;
    public Transform distToVendeta;
    public InGamePlayerManager pManager;
    public bool success;
    public bool run;
    public Collider colliderCheck;
    public Transform position;
    public Collider[] hitBox;
    public float aggroTimer;
    public List<float> damageCounts;

    // Start is called before the first frame update
    void Start()
    {
        timeShotAt = Mathf.NegativeInfinity;
        pManager = FindObjectOfType<InGamePlayerManager>();
        success = false;
        run = false;
        damageCounts = new List<float>();
    }
    private void Update()
    {
        if (aggroTimer > 0)
        {
            aggroTimer -= Time.deltaTime;
            if (aggroTimer <= 0)
            {
                vendeta = null;
                GetComponentInParent<ChargerAI>().state = ChargerAI.State.wander;
            }
        }
    }
    //Handles seeing, capped distance ray cast, currently a detection sphere
    public bool visionCheck()
    {
        //bool success = false;
        //run = true;
        position = transform;

        Collider[] self = GetComponentsInParent<Collider>();
        hitBox = self;
        RaycastHit closest = new RaycastHit();
        closest.distance = Mathf.Infinity;
        foreach (GameObject obj in pManager.players)
        {
            RectTransform p = obj.GetComponent<RectTransform>();

            if (distance(p) < visionLimit)
            {
                RaycastHit[] seen = Physics.RaycastAll(transform.position, p.position - transform.position, visionLimit);
                foreach (var next in seen)
                {
                    if (!self.Contains(next.collider) && next.distance < closest.distance)
                    {
                        run = true;
                        closest = next;
                    }
                }
            }
        }

        //front most object is a player
        //playermanager.contains(closest.collider.getComponentInParent<transform>());
        //closest.collider.GetComponentInParent<CharacterController>()
        colliderCheck = closest.collider;
        if (colliderCheck)
        {

        }
        if (closest.collider && pManager.players.Contains(closest.collider.gameObject))
        {
            success = true;
            //detected = the playerObject that hit belongs to
            detected = closest.collider.GetComponentInParent<Transform>();
        }
        
        return success;
    }
    //Handles being shot, probably an event handler in the future
    public void onDamage(float damage, GameObject shooter, int status)
    {
        if (Time.time - timeShotAt > aggroTimer)
        {
            //monster shot, first aggro, zero damage counts and set aggro of Shooter
            detected = shooter.transform;
            for (int i = 0; i < damageCounts.Count; i++)
            {
                damageCounts[i] = 0;
            }
            damageCounts[pManager.players.IndexOf(shooter)] = damage;
        }
        else
        {
            //aggro maintained, check damage counts, update damage tracker for Shooter
            damageCounts[pManager.players.IndexOf(shooter)] += damage;
        }
        //reset timer, update target to first player with highest damage count
        timeShotAt = Time.time;
        vendeta = pManager.players.ElementAt(damageCounts.IndexOf(damageCounts.Max())).transform;
    }
    private double distance(Transform other)
    {
        return Mathf.Sqrt(Mathf.Pow(transform.position.x - other.position.x, 2) + Mathf.Pow(transform.position.y - other.position.y, 2) + Mathf.Pow(transform.position.z - other.position.z, 2));
    }
}
