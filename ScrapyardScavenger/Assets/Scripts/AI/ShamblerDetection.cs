using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;

public class ShamblerDetection : MonoBehaviour
{
    public float timeShotAt;
    //in unity distance units
    public float visionLimit = 20.0F;
    public RectTransform detected;
    public AIPlayerManager pManager;
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
        pManager = FindObjectOfType<AIPlayerManager>();
        success = false;
        run = false;
        rigid = false;
        //Debug.Log("Does this even work?");
    }
    //Handles seeing, capped distance ray cast, currently a detection sphere
    //try looking for rectangle transform
    public bool visionCheck()
    {
        bool success = false;
        //run = true;
        position = transform;
        
        Collider[] self = GetComponentsInParent<Collider>();
        hitBox = self;
        RaycastHit closest = new RaycastHit();
        closest.distance = Mathf.Infinity;
        //Debug.Log("Outer loop.");
        //Debug.Log("Total Players: " + pManager.players.Count);
        foreach (var p in pManager.players)
        {
            //Debug.Log("In range.");
            if (distance(p) < visionLimit) {
                RaycastHit[] seen = Physics.RaycastAll(transform.position, p.position-transform.position, visionLimit);
                //Debug.Log(seen.Length);
                foreach (var next in seen)
                {
                   // Debug.Log("Contains:" + self.Contains(next.collider));
                    //Debug.Log("Distance:" + (next.distance < closest.distance));
                    
                    //will need to change this to rigid body later
                    if ((!self.Contains(next.collider) || next.rigidbody) && next.distance < closest.distance)
                    {
                       //Debug.Log("Collision detected");
                        //player object uses rigid body and doesn't have a collider
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

        //front most object is a player
        //playermanager.contains(closest.collider.getComponentInParent<transform>());
        //closest.collider.GetComponentInParent<CharacterController>()
        /*if (closest.collider)
        {
            Debug.Log(pManager.players.Contains(closest.collider.GetComponent<RectTransform>()));
        }
        else
        {
            Debug.Log("closet.collider null");
        }*/

        if (rigid)
        {
            if (closest.rigidbody && closest.rigidbody.detectCollisions && pManager.players.Contains(closest.rigidbody.gameObject.GetComponent<RectTransform>()))
            {
                //Debug.Log("Rigidbody detected");
                success = true;
                detected = closest.rigidbody.GetComponentInParent<RectTransform>();
            }
        }
        else
        {
            if (closest.collider && pManager.players.Contains(closest.collider.GetComponent<RectTransform>()))
            {
                //Debug.Log("Player detected");
                success = true;
                //detected = the playerObject that hit belongs to
                detected = closest.collider.GetComponentInParent<RectTransform>();
            }
        }

        
        //System.Console.WriteLine(success);
        return success;
    }
    //Handles being shot, probably an event handler in the future
    public void gotShot(GameObject shooter)
    {
        timeShotAt = Time.time;
        detected = shooter.GetComponent<RectTransform>();
    }
    private double distance(Transform other)
    {
        return Mathf.Sqrt(Mathf.Pow(transform.position.x - other.position.x, 2) + Mathf.Pow(transform.position.y - other.position.y, 2) + Mathf.Pow(transform.position.z - other.position.z, 2));
    }
}
