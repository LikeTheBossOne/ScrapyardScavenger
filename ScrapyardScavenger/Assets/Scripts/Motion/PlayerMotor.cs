using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

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

    private bool isPaused;

    private Coroutine sprintCoroutine;
    private AudioSource source;

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

        baseFOV = normalCam.fieldOfView;
        sprintFOVModifier = 1.2f;
        sprintLimit = 5; // 5 seconds
        sprintCooldown = 4; // 3 seconds

        isPaused = false;
        isEnergized = false;
        isSprinting = false;
        isCoolingDown = false;
        pastSprintPressed = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
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
        bool sprintPressed = Input.GetKey(KeyCode.LeftShift);
        bool jumpPressed = Input.GetKey(KeyCode.Space);
        

        // States
        bool isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.1f, ground);
        bool isJumping = jumpPressed && isGrounded;
        if (!isSprinting && sprintPressed && (verticalInput > 0) && !isJumping && isGrounded && !isCoolingDown)
        {
            sprintCoroutine = StartCoroutine(SprintRoutine(sprintLimit));
            // only start if this is a new sprint?
            /*if (!pastSprintPressed)
            {
                StartSprinting(sprintLimit);
            }*/
            
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

    /*public void CoolDown(int seconds)
    {
        StartCoroutine(CoolDownRoutine(seconds));
    }*/

    public IEnumerator CoolDownRoutine(int seconds)
    {
        Debug.Log("Starting cool down");
        isCoolingDown = true;
        yield return new WaitForSeconds(seconds);
        isCoolingDown = false;
        Debug.Log("Done cooling down");
    }
}
