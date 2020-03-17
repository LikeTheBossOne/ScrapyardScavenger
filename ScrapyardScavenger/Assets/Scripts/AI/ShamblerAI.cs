using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
//note: enemeyController may be useful to look at
public class ShamblerAI : MonoBehaviour
{
   public enum State
    {
        wander,
        chase,
        attack,
        spit,
        bite,
    }
    public State currentState;
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
    public ShamblerAttacks weapons;
    // Start is called before the first frame update
    private void OnEnable()
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
        weapons = GetComponent<ShamblerAttacks>();
        //playerOffset = 5;
    }
    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            changeState();
            handleState();
        }


    }
    public void changeState()
    {
        
        if (senses.visionCheck())
        {
            //Debug.Log("Range check");
            if (distanceToOther(senses.detected) <= weapons.meleeRange )
            {
                //Debug.Log("In bite range");
                //&& !weapons.meleeOnCoolDown()
                currentState = State.bite;
            }else
            if (distanceToOther(senses.detected) <= weapons.spitRange )
            {
                //Debug.Log("In spit range");
                // target in attack range/
                //currentState = State.attack;
                //&& !weapons.spitOnCoolDown()
                currentState = State.spit;
            }
            else
            {
                currentState = State.chase;
            }
            //currentState = State.chase;
        }
        else
        {
            currentState = State.wander;
        }
        //Debug.Log("State: " + currentState);
    } 
    // Update is called once per frame
    public void handleState()
    {
        if (currentState == State.chase)
        {
            //set target destination to detected/aggressing player or use follow command if there is one
            //Need to add reorientation/ "lockon camera" for enemy
            //System.Console.WriteLine("Player seen.");
            transform.LookAt(senses.detected.position, transform.up);
            setDestination(senses.detected.position);
        }
        if (currentState == State.wander)
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
            RectTransform close = findClosestPlayer();

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
        if (currentState == State.attack)
        {
            setDestination(GetComponentInParent<Transform>().position);
            gameObject.transform.LookAt(senses.detected, gameObject.transform.up);
            if (distanceToOther(senses.detected) <= weapons.meleeRange)
            {
                //target in melee range
                weapons.bite(senses.detected.gameObject);
            }
            else
            {
                //target in spit range
                weapons.spit(senses.detected.gameObject);
            }
        }
        if (currentState == State.spit)
        {
            setDestination(GetComponentInParent<Transform>().position);
            gameObject.transform.LookAt(senses.detected, gameObject.transform.up);
            weapons.spit(senses.detected.gameObject);
        }
        if (currentState == State.bite)
        {
            setDestination(GetComponentInParent<Transform>().position);
            gameObject.transform.LookAt(senses.detected, gameObject.transform.up);
            weapons.bite(senses.detected.gameObject);
        }
        
    }

    //Finds the closest player
    RectTransform findClosestPlayer()
    {
        RectTransform closest = null;
        double cDist = Mathf.Infinity;
        //Find closest player or vehicle
        foreach (RectTransform ally in players.players)
        {
            double dist = distanceToOther(ally);
            if ( dist < cDist)
            {
                closest = ally;
                cDist = dist;
            }
        }
        return closest;
    }
    void setDestination(Vector3 destination)
    {
        nav.SetDestination(destination);
    }

    double distanceToOther(RectTransform other)
    {        
        return Mathf.Sqrt(Mathf.Pow(other.position.x - transform.position.x,2) + Mathf.Pow(other.position.y- transform.position.y,2) + Mathf.Pow(other.position.z-transform.position.z,2));
    }
    
}
