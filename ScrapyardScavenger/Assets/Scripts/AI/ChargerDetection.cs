﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChargerDetection : MonoBehaviour
{
    public float timeShotAt;
    //in unity distance units
    public float visionLimit = 20.0F;
    public Transform detected;
    public AIPlayerManager pManager;
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
        pManager = FindObjectOfType<AIPlayerManager>();
        success = false;
        run = false;
        damageCounts = new List<float>();
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
        Debug.Log("Outer loop.");
        foreach (var p in pManager.players)
        {
            Debug.Log("In range.");
            if (distance(p) < visionLimit)
            {
                RaycastHit[] seen = Physics.RaycastAll(transform.position, p.position - transform.position, visionLimit);
                Debug.Log(seen.Length);
                foreach (var next in seen)
                {
                    Debug.Log("Contains:" + self.Contains(next.collider));
                    Debug.Log("Distance:" + (next.distance < closest.distance));
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
    private void onDamage(float damage, GameObject shooter, int status)
    {
        if (Time.time - timeShotAt > aggroTimer)
        {
            detected = shooter.transform;
            damageCounts.Clear();
            damageCounts.Insert(pManager.players.IndexOf(shooter.transform), damage);
        }
        else
        {
            if (damageCounts.Capacity < pManager.)
            {

            }
            damageCounts.+= damage;
            damageCounts.
            detected = pManager.players.ElementAt(damageCounts.First(damageCounts.Max()).transform;
        }
        timeShotAt = Time.time;
        
    }
    private double distance(Transform other)
    {
        return Mathf.Sqrt(Mathf.Pow(transform.position.x - other.position.x, 2) + Mathf.Pow(transform.position.y - other.position.y, 2) + Mathf.Pow(transform.position.z - other.position.z, 2));
    }
}
