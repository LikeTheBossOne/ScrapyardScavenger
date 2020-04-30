using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviourPunCallbacks
{
    public float speed;
    public float speedModifier;
    public float sprintModifier;
    private int sprintLimit; // how long the player can sprint for!
    private int sprintCooldown; // how long the cooldown is between sprints
    private bool pastSprintPressed;

    public float jumpForce;
    public Camera normalCam;
    public GameObject cameraParent;
    public Transform groundDetector;
    public LayerMask ground;


    private Rigidbody myRigidbody;
    private float baseFOV;
    private float sprintFOVModifier;
    private bool isEnergized;
    private bool isSprinting;
    private bool isCoolingDown;
    private float deadZone;

    public Animator animator;

    private bool isPaused;

    private Coroutine sprintCoroutine;

    private AudioSource source;

    private bool justFell;

    void Start()
    {
        if (photonView.IsMine)
        {
            cameraParent.SetActive(true);
        }

        Camera.main.enabled = false;
        myRigidbody = GetComponent<Rigidbody>();
        source = GetComponent<AudioSource>();
        Debug.Log(source);

        normalCam.gameObject.SetActive(true);
        baseFOV = normalCam.fieldOfView;
        sprintFOVModifier = 1.2f;
        sprintLimit = 5; // 5 seconds
        sprintCooldown = 4; // 3 seconds

        isPaused = false;
        isEnergized = false;
        isSprinting = false;
        isCoolingDown = false;
        pastSprintPressed = false;
        //animator = GetComponentInChildren<Animator>();

        deadZone = 0.01f;
        justFell = false;
        // check to see if the player has the Endurance skill?
        SkillLevel enduranceLevel = GetComponent<PlayerControllerLoader>().skillManager.GetSkillByName("Endurance");
        if (enduranceLevel != null)
        {
            // they have it, so set it
            sprintLimit = (int) enduranceLevel.Modifier;
            Debug.Log("Player has Endurance, modifier is: " + sprintLimit);
        }
        else Debug.Log("Player does NOT have Endurance");

    }

    void Update()
    {
        // If not me, don't update!
        if (!photonView.IsMine) return;

        if (myRigidbody == null)
        {
            myRigidbody = GetComponent<Rigidbody>();
        }

        if (source == null)
        {
            source = GetComponent<AudioSource>();
        }

        if (source == null || myRigidbody == null) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
        }
        if ((myRigidbody.velocity.magnitude > .1) && (myRigidbody.velocity.y < .1 && myRigidbody.velocity.y > -.1) && !source.isPlaying)
        {
            source.volume = .03f;
            source.Play();
        }
        if (((myRigidbody.velocity.magnitude < .1) || (myRigidbody.velocity.y > .1 || myRigidbody.velocity.y < -.1)) && source.isPlaying)
        {
            source.volume -= .005f;
            if (source.volume == 0)
            {
                source.Stop();
            }
        }
    }

    void FixedUpdate()
    {
        // If not me, don't update!
        if (!photonView.IsMine) return;

        if (!isPaused)
        {
            Move();

        }
    }


    void OnApplicationFocus(bool hasFocus)
    {
        // If not me, don't update!
        if (!photonView.IsMine) return;


        isPaused = !hasFocus;
    }

    void Move()
    {
        // Inputs
        float verticalInput = Input.GetAxisRaw("Vertical");
        float horizontalInput = Input.GetAxisRaw("Horizontal");
		bool sprintPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKeyDown("joystick button 8");
		bool jumpPressed = Input.GetKey(KeyCode.Space) || Input.GetKeyDown("joystick button 3");


        // States
        bool isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.1f, ground);
        bool isJumping = jumpPressed && isGrounded;
        if (!isSprinting && sprintPressed && (verticalInput > 0) && !isJumping && isGrounded && !isCoolingDown)
        {
            sprintCoroutine = StartCoroutine(SprintRoutine(sprintLimit));

        }
        else
        {
            if (pastSprintPressed && !sprintPressed && !isCoolingDown)
            {
                // start cool down?
                isSprinting = false;
                Debug.Log("Begin cool down in Move method");
                StopCoroutine(sprintCoroutine);
                StartCoroutine(CoolDownRoutine(sprintCooldown));
            }
        }


        // Jumping
        if (isJumping)
        {
            myRigidbody.AddForce(Vector3.up * jumpForce);

        }


        // Movement
        Vector3 direction = new Vector3(horizontalInput, 0, verticalInput);
        direction.Normalize();

        float adjustedSpeed = speed;
        if (isSprinting)
        {
            adjustedSpeed *= sprintModifier;
        }
        if (isEnergized)
        {
            adjustedSpeed *= speedModifier;
        }

        Vector3 targetVelocity = transform.TransformDirection(direction) * adjustedSpeed * Time.fixedDeltaTime;
        targetVelocity.y = myRigidbody.velocity.y;
        myRigidbody.velocity = targetVelocity;


        // Sprinting FOV
        normalCam.fieldOfView = isSprinting
            ? Mathf.Lerp(normalCam.fieldOfView, baseFOV * sprintFOVModifier, Time.fixedDeltaTime * 8f)
            : Mathf.Lerp(normalCam.fieldOfView, baseFOV, Time.fixedDeltaTime * 2f);

        if (animator)
        {
            //might need to move this out of fixedUpdate
            if (isGrounded)
            {
                if (!justFell)
                {
                    justFell = true;
                    photonView.RPC("Land", RpcTarget.All);
                    //animator.SetBool("Grounded", true);
                }
                else if (!isJumping)
                {
                    //if (isSprinting)
                    //{
                    //    gameObject.GetPhotonView().RPC("Run", RpcTarget.All);
                    //}
                    //else if (Mathf.Abs(verticalInput) > deadZone || Mathf.Abs(horizontalInput) > deadZone)
                    //{
                    //    gameObject.GetPhotonView().RPC("Walk", RpcTarget.All);
                    //}
                    //else
                    //{
                    //    gameObject.GetPhotonView().RPC("Idle", RpcTarget.All);
                    //}
                    if (isSprinting || Mathf.Abs(verticalInput) > deadZone || Mathf.Abs(horizontalInput) > deadZone)
                    {
                        photonView.RPC("Move", RpcTarget.All, adjustedSpeed);
                    }
                    else
                    {
                        photonView.RPC("Idle", RpcTarget.All);
                    }
                }

            }
            else if (isJumping)
            {
                //animator.SetBool("Jump", true);
                photonView.RPC("Jump", RpcTarget.All);
            }
            else if(justFell)
            {
                //animator.SetBool("Grounded", false);
                photonView.RPC("Fall", RpcTarget.All);
                justFell = false;
            }

        }


        pastSprintPressed = sprintPressed;
    }

    public void Energize(int seconds)
    {
        StartCoroutine(EnergizeRoutine(seconds));
    }

    public IEnumerator EnergizeRoutine(int seconds)
    {
        isEnergized = true;
        yield return new WaitForSeconds(seconds);
        Unenergize();
    }

    public void Unenergize() {
        isEnergized = false;
    }

    public IEnumerator SprintRoutine(int seconds)
    {
        Debug.Log("Sprinting for " + seconds + " seconds");
        isSprinting = true;
        yield return new WaitForSeconds(seconds);
        if (isSprinting)
        {
            // still sprinting, so stop
            Debug.Log("Stop sprinting in routine");
            isSprinting = false;
            StartCoroutine(CoolDownRoutine(sprintCooldown));
        }
        Debug.Log("Finished sprinting routine");
    }

    public void SetSprintLimit(int limit)
    {
        sprintLimit = limit;
    }

    public IEnumerator CoolDownRoutine(int seconds)
    {
        Debug.Log("Starting cool down");
        isCoolingDown = true;
        yield return new WaitForSeconds(seconds);
        isCoolingDown = false;
        Debug.Log("Done cooling down");
    }

    public void JumpEnd()
    {
        if (animator != null)
        {
            animator.SetBool("Jump", false);
        }
    }

    [PunRPC]
    public void Walk()
    {
        if (animator != null)
        {
            animator.SetBool("walk", true);
            animator.SetBool("run", false);
        }
    }

    [PunRPC]
    public void Run()
    {
        if (animator != null)
        {
            animator.SetBool("walk", false);
            animator.SetBool("run", true);
        }
    }

    [PunRPC]
    public void Idle()
    {
        //animator.SetBool("walk", false);
        //animator.SetBool("run", false);

        if (animator != null)
        {
            animator.SetBool("Idle", true);
        }
        
    }

    [PunRPC]

    public void Move( float spd)
    {
        if (animator != null)
        {
            animator.SetBool("Idle", false);
        }

        float calculated = spd / (speed * sprintModifier);
        if (calculated > 1)
        {
            calculated = 1;
        }


        if (animator != null)
        {
            animator.SetFloat("speed", calculated);
        }
        
    }

    [PunRPC]
    public void Jump()
    {

        if (animator != null)
        {
            animator.SetBool("Jump", true);
        }
        
    }

    [PunRPC]
    public void Land()
    {

        if (animator != null)
        {
            animator.SetBool("Grounded", true);
            animator.SetBool("Jump", false);
        }
        
    }

    [PunRPC]
    public void Fall()
    {
        if (animator != null)
        {
            animator.SetBool("Grounded", false);
        }
       
    }


    [PunRPC]
    public void StartHeal()
    {
        Debug.Log("Med animation start");
        //gameObject.GetComponentInChildren<Gun>().;
        transform.GetChild(1).gameObject.SetActive(false);
        GetComponentInChildren<GunIkController>().IkActive = false;
        if(animator != null){
          animator.SetBool("Medkit", true);
        }
        

    }
}
