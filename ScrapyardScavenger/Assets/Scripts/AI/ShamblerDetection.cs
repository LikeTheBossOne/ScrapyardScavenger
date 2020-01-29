using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShamblerDetection : MonoBehaviour
{
    public float timeShotAt;
    //in unity distance units
    public float visionLimit = 60.0F;
    public Transform detected;
    public PlayerManager pManager;
    public bool success;
    public bool run;
    public Collider colliderCheck;
    public Transform position;
    public Collider[] hitBox;
    
    // Start is called before the first frame update
    void Start()
    {
        timeShotAt = Mathf.NegativeInfinity;
        pManager = FindObjectOfType<PlayerManager>();
        success = false;
        run = false;
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
        foreach (var p in pManager.players)
        {
            if (distance(p) < visionLimit) {
                RaycastHit[] seen = Physics.RaycastAll(transform.position, p.position-transform.position, visionLimit);
                foreach (var next in seen)
                {
                    if (self.Contains(next.collider) && next.distance < closest.distance)
                    {
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
            run = true;
        }
        if (closest.collider && pManager.players.Contains(closest.collider.GetComponent<Transform>()))
        {
            success = true;
            //detected = the playerObject that hit belongs to
            detected = closest.collider.GetComponentInParent<Transform>();
        }
        System.Console.WriteLine(success);
        return success;
    }
    //Handles being shot, probably an event handler in the future
    private void gotShot(GameObject shooter)
    {
        timeShotAt = Time.time;
        detected = shooter.transform;
    }
    private double distance(Transform other)
    {
        return Mathf.Sqrt(Mathf.Pow(transform.position.x - other.position.x, 2) + Mathf.Pow(transform.position.y - other.position.y, 2) + Mathf.Pow(transform.position.z - other.position.z, 2));
    }
}
