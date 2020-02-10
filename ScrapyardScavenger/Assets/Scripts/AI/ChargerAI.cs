using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//note: enemeyController may be useful to look at
public class ChargerAI : MonoBehaviour
{
    public Vector3 moveTo;
    public NavMeshAgent nav;
    public AIPlayerManager players;
    //intent, second based countdown
    //public int resetDelay = 600;
    //private int timer;
    public float aggroTimeLimit;
    public float wandOffset;
    public float toPlayerOffset;
    public float wandAngle;
    public float wandRad;
    //public float playerOffset;
    public ShamblerDetection senses;
    // Start is called before the first frame update
    void Start()
    {
        //timer = 0;
        aggroTimeLimit = 10;
        senses = GetComponent<ShamblerDetection>();
        nav = GetComponentInParent<NavMeshAgent>();
        players = FindObjectOfType<AIPlayerManager>();
        wandOffset = 10;
        toPlayerOffset = 20;
        wandAngle = 60;
        wandRad = 10;
        //playerOffset = 5;
    }

    // Update is called once per frame
    void Update()
    {
        //key decision points: player detected, unit shot recently
        //Time.time-senses.timeShotAt <= aggroTimeLimit ||
        senses.visionCheck();
        //System.Console.WriteLine(senses.success);
        if (senses.detected)
        {
            //set target destination to detected/aggressing player or use follow command if there is one
            //Need to add reorientation/ "lockon camera" for enemy
            //System.Console.WriteLine("Player seen.");
            transform.LookAt(senses.detected.position, transform.up);
            setDestination(senses.detected.position);
        }
        else
        {
            //wander in the direction of closest player
            //need to rethink this with unity in mind
            //need to rethink transform.rotate()
            Vector3 wandDir = (transform.forward - transform.position).normalized;
            //project the center of the imaginary circle
            Vector3 center = wandDir * wandOffset;
            center = center + transform.position;
            //orient
            float angle = Random.Range(-1, 1) * wandAngle;
            //transform.Rotate(transform.up, angle);
            float curOrient = transform.eulerAngles.y;
            float newOrient = curOrient + angle;
            Vector3 dir = transform.forward.normalized;
            dir.x = Mathf.Sin(newOrient);
            dir.z = Mathf.Cos(newOrient);
            //project target spot on circle
            Vector3 moveTarg = center + wandRad * dir;
            //correct towards closest player
            Transform close = findClosestPlayer();

            Vector3 toPlayer = close.position - moveTarg;
            toPlayer = toPlayer.normalized;
            if (distanceToOther(close) < toPlayerOffset)
            {
                toPlayer = toPlayer * (float)distanceToOther(close);
            }
            else
            {
                toPlayer = toPlayer * toPlayerOffset;
            }

            moveTarg = moveTarg + toPlayer;
            //this line is why the blue sphere warps around
            //close.position = moveTarg;
            //transform.LookAt(moveTarg, transform.up);
            moveTo = moveTarg;
            setDestination(moveTo);
        }
    }

    //Finds the closest player
    Transform findClosestPlayer()
    {
        Transform closest = null;
        double cDist = Mathf.Infinity;
        //Find closest player or vehicle
        foreach (Transform ally in players.players)
        {
            double dist = distanceToOther(ally.transform);
            if (dist < cDist)
            {
                closest = ally.transform;
                cDist = dist;
            }
        }
        return closest;
    }
    void setDestination(Vector3 destination)
    {
        nav.SetDestination(destination);
    }

    double distanceToOther(Transform other)
    {
        return Mathf.Sqrt(Mathf.Pow(other.position.x - transform.position.x, 2) + Mathf.Pow(other.position.y - transform.position.y, 2) + Mathf.Pow(other.position.z - transform.position.z, 2));
    }

}
