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
    public float jumpForce;
    public Camera normalCam;
    public GameObject cameraParent;
    public Transform groundDetector;
    public LayerMask ground;

    private Rigidbody myRigidbody;
    private float baseFOV;
    private float sprintFOVModifier;
    private bool isEnergized;

    private bool isPaused;

    void Start()
    {
        if (photonView.IsMine)
        {
            cameraParent.SetActive(true);
        }

        Camera.main.enabled = false;

        myRigidbody = GetComponent<Rigidbody>();

        baseFOV = normalCam.fieldOfView;
        sprintFOVModifier = 1.2f;

        isPaused = false;
        isEnergized = false;
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
        bool isSprinting = sprintPressed && (verticalInput > 0) && !isJumping && isGrounded;


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
}
