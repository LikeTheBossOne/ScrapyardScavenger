using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;

public class ShamblerDetection : MonoBehaviour
{
    public float timeShotAt;
    // in unity distance units
    public float visionLimit = 20.0F;
    public Transform detected;
    public InGamePlayerManager pManager;
    public bool success;
    public bool run;
    public Collider colliderCheck;
    public Transform position;
    public Collider[] hitBox;
    private bool rigid;
    public Transform extractionTruck;
    private Transform TruckAttackPoints;

    // Start is called before the first frame update
    private void OnEnable()
    {
        timeShotAt = Mathf.NegativeInfinity;
        pManager = FindObjectOfType<InGamePlayerManager>();
        success = false;
        run = false;
        rigid = false;
        extractionTruck = GameObject.Find("ExtractionTruck").GetComponent<Transform>();
        for (int i = 0; i < extractionTruck.childCount; i++)
        {
            if (extractionTruck.GetChild(i).name == "AttackPoints")
            {
                TruckAttackPoints = extractionTruck.GetChild(i);
                break;
            }
        }
        if (TruckAttackPoints == null) Debug.Log("Could not find AttackPoints object on truck");
        
    }
    // Handles seeing, capped distance ray cast, currently a detection sphere
    // try looking for rectangle transform
    public bool VisionCheck()
    {
        bool success = false;
        //run = true;
        position = transform;
        
        Collider[] self = GetComponentsInParent<Collider>();
        hitBox = self;
        RaycastHit closest = new RaycastHit();
        closest.distance = Mathf.Infinity;
        foreach (GameObject obj in pManager.players)
        {
            if (obj == null) continue;

            Transform p = obj.GetComponent<Transform>();

            if (distance(p) < visionLimit) {
                RaycastHit[] seen = Physics.RaycastAll(transform.position, p.position-transform.position, visionLimit);
                foreach (var next in seen)
                {
                    
                    // will need to change this to rigid body later
                    if ((!self.Contains(next.collider) || next.rigidbody) && next.distance < closest.distance)
                    {
                        // player object uses rigid body and doesn't have a collider
                        closest = next;
                        if (closest.rigidbody)
                        {
                            rigid = true;
                        }
                        else
                        {
                            rigid = false;
                        }
                    }
                }
            }
        }

        if (rigid)
        {
            if (closest.rigidbody && closest.rigidbody.detectCollisions && pManager.players.Contains(closest.rigidbody.gameObject))
            {
                success = true;
                detected = closest.rigidbody.GetComponentInParent<Transform>();
            }
        }
        else
        {
            if (closest.collider && pManager.players.Contains(closest.collider.gameObject))
            {
                success = true;
                //detected = the playerObject that hit belongs to
                detected = closest.collider.GetComponentInParent<Transform>();
            }
        }

        if (distance(extractionTruck) < visionLimit)
        {
            RaycastHit[] seen = Physics.RaycastAll(transform.position, extractionTruck.position - transform.position, visionLimit);
            foreach (var next in seen)
            {
                // will need to change this to rigid body later
                if ((!self.Contains(next.collider) || next.rigidbody) && next.distance < closest.distance)
                {
                    // player object uses rigid body and doesn't have a collider
                    closest = next;
                    if (closest.rigidbody)
                    {
                        rigid = true;
                    }
                    else
                    {
                        rigid = false;
                    }
                }
            }

            if (rigid)
            {
                if (closest.rigidbody && closest.rigidbody.detectCollisions && GameObject.Find("ExtractionTruck").transform == (closest.rigidbody.gameObject.transform))
                {
                    success = true;
                    //detected = closest.rigidbody.GetComponentInParent<Transform>();
                    detected = GetClosestPointOnTruck();
                }
            }
            else
            {
                if (closest.collider && GameObject.Find("ExtractionTruck").transform == (closest.collider.gameObject.transform))
                {
                    success = true;
                    //detected = the playerObject that hit belongs to
                    detected = GetClosestPointOnTruck();
                    //detected = closest.collider.GetComponentInParent<Transform>();
                    //detected = GetClosestPointOnTruck(closest.collider.transform.parent.gameObject);
                }
            }
        }

       
        return success;
    }

    private Transform GetClosestPointOnTruck()
    {
        Transform closestPoint = null;
        double closestDistance = Mathf.Infinity;
        double distanceToPoint = 0;
        for (int i = 0; i < TruckAttackPoints.childCount; i++)
        {
            distanceToPoint = distance(TruckAttackPoints.GetChild(i));
            if (distanceToPoint < closestDistance)
            {
                closestPoint = TruckAttackPoints.GetChild(i);
                closestDistance = distanceToPoint;
            }
        }
        return closestPoint;
    }

    public void GotShot(GameObject shooter)
    {
        timeShotAt = Time.time;
        detected = shooter.GetComponent<Transform>();
    }

    private double distance(Transform other)
    {
        return Mathf.Sqrt(Mathf.Pow(transform.position.x - other.position.x, 2) + Mathf.Pow(transform.position.y - other.position.y, 2) + Mathf.Pow(transform.position.z - other.position.z, 2));
    }

    public bool PlayersExist()
    {
        return pManager.players.Count > 0;
    }
}
