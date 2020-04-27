using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
//note: enemeyController may be useful to look at
public class ShamblerAI : MonoBehaviourPun
{
    public enum State
    {
        idle,
        wander,
        chase,
        spit,
        bite,
        dead,
        Invalid,
   }

    public State lastState;
    public State currentState;
    public Vector3 moveTo;
    public NavMeshAgent nav;
    public InGamePlayerManager pManager;
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
    public Animator animator;
    public float maxSpd;
    public Transform extractionTruck;
    private Coroutine spitTimingCoroutine;
    private bool isSpitting;

    // Start is called before the first frame update
    private void OnEnable()
    {
        //timer = 0;
        aggroTimeLimit = 10;
        senses = GetComponent<ShamblerDetection>();
        nav = GetComponentInParent<NavMeshAgent>();
        pManager = FindObjectOfType<InGamePlayerManager>();
        wandOffset = 10;
        toPlayerOffset = 20;
        wandAngle = 60;
        wandRad = 10;
        weapons = GetComponent<ShamblerAttacks>();
        animator = GetComponentInChildren<Animator>();
        maxSpd = GetComponent<NavMeshAgent>().speed;
        isSpitting = false;
        extractionTruck = GameObject.Find("ExtractionTruck").GetComponent<Transform>();

        // delete this after figuring out why the shambler won't move to the truck sometimes
        moveTo = gameObject.transform.position;
        //playerOffset = 5;
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && currentState != State.dead)
        {
            if (animator.GetBool("Spit"))
            {
                LookAtDetected();
                return;
            }
            ChangeState();
            HandleState();
        }
    }

    

    public void ChangeState()
    {
        lastState = currentState;
        if (senses.PlayersExist())
        {
            if (senses.VisionCheck())
            {
                double distanceToDetected = DistanceToOther(senses.detected);
                if (distanceToDetected <= weapons.meleeRange && !weapons.MeleeOnCoolDown())
                {
                    //if (lastState != State.bite) Debug.Log("Switching to Bite state");
                    currentState = State.bite;
                }
                else if (distanceToDetected <= weapons.spitRange && !weapons.SpitOnCoolDown())
                {
                    //if (lastState != State.spit) Debug.Log("Switching to Spit state");
                    currentState = State.spit;
                }
                else if (distanceToDetected <= weapons.meleeRange)
                {
                    // just go idle then
                    //if (lastState != State.idle) Debug.Log("Switching to Idle state");
                    currentState = State.idle;
                }
                else
                {
                    //if (lastState != State.chase) Debug.Log("Switching to Chase state");
                    currentState = State.chase;

                }
            }
            else
            {
                //if (lastState != State.wander) Debug.Log("Switching to Wander state");
                currentState = State.wander;

            }
        }
        else
        {
            //if (lastState != State.idle) Debug.Log("Switching to Idle state");
            currentState = State.idle;
        }
    }

    // Update is called once per frame
    public void HandleState()
    {

        if (currentState == State.chase)
        {
            //set target destination to detected/aggressing player or use follow command if there is one
            //Need to add reorientation/ "lockon camera" for enemy
            LookAtDetected();
            moveTo = senses.detected.position;
            MoveToDetected();

            if (animator && currentState != lastState)
            {
                photonView.RPC("Walk", RpcTarget.All);
            }
        }
        if (currentState == State.wander && currentState != lastState)
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
            Transform close = FindClosestPlayer();

            Vector3 toPlayer = close.position - moveTarg;
            toPlayer = toPlayer.normalized;
            if (DistanceToOther(close) < toPlayerOffset)
            {
                toPlayer = toPlayer * (float)DistanceToOther(close);
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

            MoveToDetected();
            
            if (animator)
                photonView.RPC("Walk", RpcTarget.All);
        }
        if (currentState == State.spit)
        {
            LookAtDetected();

            if (currentState != lastState)
            {
                //SetDestination(senses.detected.position); << this happens in the Spit method

                if (animator)
                {
                    if (!animator.GetBool("Spit"))
                    {
                        photonView.RPC("Spit", RpcTarget.All);
                    }
                }
                else
                {
                    weapons.Spit(senses.detected.gameObject);
                }
            }


        }
        if (currentState == State.bite && currentState != lastState)
        {
            SetDestination(GetComponentInParent<Transform>().position);
            LookAtDetected();
            
            /*if (animator && !animator.GetBool("Spit"))
                photonView.RPC("Spit", RpcTarget.All);*/

            if (senses.detected.gameObject.tag == "Player")
                weapons.Bite(senses.detected.gameObject);
            else // this is a point on the truck, so pass in the truck's gameObject
                weapons.Bite(extractionTruck.gameObject);
        }
        if (currentState == State.idle && currentState != lastState)
        {
            SetDestination(gameObject.transform.position);
            if (animator && !animator.GetBool("Spit"))
            {
                photonView.RPC("Idle", RpcTarget.All);
            }
        }

    }

    private void MoveToDetected()
    {
        if (moveTo.y < 0)
        {
            Vector3 temp = moveTo;
            temp.y = 0f;
            moveTo = temp;
        }

        SetDestination(moveTo);
    }

    private void LookAtDetected()
    {
        Vector3 lookSpot = senses.detected.position;
        lookSpot.y = gameObject.transform.position.y;
        gameObject.transform.LookAt(lookSpot, gameObject.transform.up);
    }

    Transform FindClosestPlayer()
    {
        Transform closest = null;
        double cDist = Mathf.Infinity;

        // Find closest player or vehicle
        foreach (GameObject obj in pManager.players)
        {
            if (obj == null) continue;

            Transform player = obj.GetComponent<Transform>();

            double dist = DistanceToOther(player);
            if (dist < cDist)
            {
                closest = player;
                cDist = dist;
            }
        }
        if (DistanceToOther(extractionTruck) < cDist)
        {
            return extractionTruck;
        }
        return closest;
    }

    public void SetDestination(Vector3 destination)
    {
        nav.SetDestination(destination);
    }

    double DistanceToOther(Transform other)
    {
        return Mathf.Sqrt(Mathf.Pow(other.position.x - transform.position.x, 2) + Mathf.Pow(other.position.y - transform.position.y, 2) + Mathf.Pow(other.position.z - transform.position.z, 2));
    }

    [PunRPC]
    public void Walk()
    {
        animator.SetBool("walking", true);
        //animator.SetFloat("speed", spd);
    }

    [PunRPC]
    public void Idle()
    {
        animator.SetBool("walking", false);
    }

    [PunRPC]
    public void Die()
    {
        SetDestination(GetComponentInParent<Transform>().position);
        currentState = State.dead;
        if (animator)
        {
            animator.SetBool("Dead", true);
        }

        GetComponent<CapsuleCollider>().enabled = false;
        foreach (var collider in GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
    }

    [PunRPC]
    public void Spit()
    {
        animator.SetBool("Spit", true);
    }

    [PunRPC]
    public void EndSpit()
    {
        animator.SetBool("Spit", false);
    }

}
