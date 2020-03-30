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
    public RectTransform detected;
    public InGamePlayerManager pManager;
    public bool success;
    public bool run;
    public Collider colliderCheck;
    public Transform position;
    public Collider[] hitBox;
    private bool rigid;
    
    // Start is called before the first frame update
    private void OnEnable()
    {
        timeShotAt = Mathf.NegativeInfinity;
        pManager = FindObjectOfType<InGamePlayerManager>();
        success = false;
        run = false;
        rigid = false;
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
            RectTransform p = obj.GetComponent<RectTransform>();

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
                detected = closest.rigidbody.GetComponentInParent<RectTransform>();
            }
        }
        else
        {
            if (closest.collider && pManager.players.Contains(closest.collider.gameObject))
            {
                success = true;
                //detected = the playerObject that hit belongs to
                detected = closest.collider.GetComponentInParent<RectTransform>();
            }
        }
        return success;
    }

    public void GotShot(GameObject shooter)
    {
        timeShotAt = Time.time;
        detected = shooter.GetComponent<RectTransform>();
    }

    private double distance(Transform other)
    {
        return Mathf.Sqrt(Mathf.Pow(transform.position.x - other.position.x, 2) + Mathf.Pow(transform.position.y - other.position.y, 2) + Mathf.Pow(transform.position.z - other.position.z, 2));
    }
}
