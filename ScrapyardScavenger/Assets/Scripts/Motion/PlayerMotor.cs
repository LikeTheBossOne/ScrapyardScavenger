using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
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
    public int homebaseIndex;

    public float jumpForce;
    public Camera normalCam;
    public GameObject cameraParent;
    public Transform groundDetector;
    public LayerMask ground;
    private GameObject evacuateCanvas;
    private GameObject truck;

    private Rigidbody myRigidbody;
    private float baseFOV;
    private float sprintFOVModifier;
    private bool isEnergized;
    private bool isSprinting;
    private bool isCoolingDown;
    private bool isLookingAtTruck;
    private bool isLeaving;
    private bool isPaused;

    private float leaveRadius;

    private Coroutine sprintCoroutine;
    private Coroutine leaveCoroutine;
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
        isLookingAtTruck = false;
        isLeaving = false;
        leaveRadius = 7.0f;

        evacuateCanvas = GameObject.Find("Exit Canvas");
        truck = GameObject.Find("ExtractionTruck");
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

            // check if the player is looking at the truck
            if (CheckTruck())
            {
                // show button pop up
                evacuateCanvas.GetComponentInChildren<Text>().text = "Press B to escape!";
                isLookingAtTruck = true;

                // wait for user to press the button?
            }
            else
            {
                // remove the button pop up
                evacuateCanvas.GetComponentInChildren<Text>().text = ""; // SetActive(false);
                isLookingAtTruck = false;
            }

            // now check to see if the player is trying to escape
            if (Input.GetKeyDown(KeyCode.B) && isLookingAtTruck && !isLeaving)
            {
                // draw the circle
                isLeaving = true;
                RenderCircle();

                // start the countdown
                leaveCoroutine = StartCoroutine(LeaveGame());
            }

            //Debug.Log("Distance to truck: " + Vector3.Distance(truck.transform.position, transform.position));
            if (isLeaving)
            {
                // check if the player has left the escape circle
                // by calculating the distance between the player and the truck
                float dist = Vector3.Distance(truck.transform.position, transform.position);
                if (dist > (leaveRadius + 0.5f))
                {
                    // outside of the circle?
                    // cancel the leaving
                    Debug.Log("Cancel leave");
                    isLeaving = false;
                    StopCoroutine(leaveCoroutine);

                    // turn the linerenderer off
                    truck.GetComponent<LineRenderer>().enabled = false;
                }
            }


        }
    }

    private IEnumerator LeaveGame()
    {
        Debug.Log("Leaving Game");
        float time = 0;
        float totalWaitTime = 7;
        while (time < totalWaitTime)
        {
            Debug.Log($"{totalWaitTime - time}");
            time++;
            yield return new WaitForSeconds(1);
        }
        isLeaving = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        truck.GetComponent<LineRenderer>().enabled = false;
        PhotonNetwork.LoadLevel(homebaseIndex);
    }

    public void RenderCircle()
    {
        float radius = leaveRadius; //7.5f;
        int numSegments = 128;

        LineRenderer lineRenderer = truck.GetComponent<LineRenderer>();
        if (!lineRenderer.enabled) lineRenderer.enabled = true;
        Color c1 = new Color(1.0f, 0f, 0f, 1);
        //lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.startColor = c1;
        lineRenderer.endColor = c1;
        lineRenderer.startWidth = 0.5f;
        lineRenderer.endWidth = 0.5f;
        lineRenderer.positionCount = numSegments + 1;
        lineRenderer.useWorldSpace = false;

        float deltaTheta = (float)(2.0 * Mathf.PI) / numSegments;
        float theta = 0f;

        for (int i = 0; i < numSegments + 1; i++)
        {
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);
            Vector3 pos = new Vector3(x, 0, z);
            lineRenderer.SetPosition(i, pos);
            theta += deltaTheta;
        }
    }

    bool CheckTruck()
    {
        LayerMask layerMask = LayerMask.GetMask("Truck");

        // This would cast rays only against colliders in layer 12.
        Transform eyeCam = transform.Find("Cameras/Main Player Cam");
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(eyeCam.position, eyeCam.forward, out hit, 2.5f, layerMask))
        {
            return true;
        }
        else
        {
            return false;
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

    public IEnumerator CoolDownRoutine(int seconds)
    {
        Debug.Log("Starting cool down");
        isCoolingDown = true;
        yield return new WaitForSeconds(seconds);
        isCoolingDown = false;
        Debug.Log("Done cooling down");
    }

}
