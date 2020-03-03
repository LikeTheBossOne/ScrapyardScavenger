using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using System.IO;
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
    private ExtractHelp extractionHelp;

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
    
    private bool isPaused;

    private Coroutine sprintCoroutine;
    
    private AudioSource source;

    void Start()
    {
        if (photonView.IsMine)
        {
            cameraParent.SetActive(true);
        }

        extractionHelp = GetComponent<ExtractHelp>();

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
            ExtractionCheck();
            
        }
    }

    private void ExtractionCheck()
    {
        if (LookingAtTruck() && !extractionHelp.IsOtherPlayerLeaving() && !extractionHelp.IsLeaving())
        {
            // show button pop up
            evacuateCanvas.GetComponentInChildren<Text>().text = "Press B to escape!";
            isLookingAtTruck = true;
        }
        // if both players are not leaving
        else if (!extractionHelp.IsLeaving() && !extractionHelp.IsOtherPlayerLeaving())
        {
            // remove the button pop up
            evacuateCanvas.GetComponentInChildren<Text>().text = ""; // SetActive(false);
            isLookingAtTruck = false;
        }

        if (extractionHelp.IsOtherPlayerLeaving() && !extractionHelp.IsLeaving())
        {
            // the other player wants to leave and you are not ready yet, so
            // check to see if you are within the circle or not
            // by calculating the distance between the player and the truck
            float dist = Vector3.Distance(truck.transform.position, transform.position);
            if (dist <= (extractionHelp.leaveRadius + 0.5f))
            {
                // inside the circle so notify the other player they can leave
                GetComponent<PlayerManager>().getOtherPlayer().GetPhotonView().RPC("SecondPlayerReadyToLeave", RpcTarget.All);
                photonView.RPC("SecondPlayerReadyToLeave", RpcTarget.All);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.B) && isLookingAtTruck && !extractionHelp.IsLeaving())
            {
                // if there are 2 players, signal the other player that you are leaving
                if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                {
                    GetComponent<PlayerManager>().getOtherPlayer().GetPhotonView().RPC("FirstPlayerReadyToLeave", RpcTarget.All);
                    extractionHelp.IAmReadyToLeave();

                    // reset the text
                    evacuateCanvas.GetComponentInChildren<Text>().text = "Waiting for other player";

                }

                // if only 1 player, just start the countdown
                else
                {
                    extractionHelp.SoloLeave();
                }
            }
        }




        // now check to see if the player is trying to escape from the circle
        if (extractionHelp.IsLeaving())
        {
            // check if the player has left the escape circle
            // by calculating the distance between the player and the truck
            float dist = Vector3.Distance(truck.transform.position, transform.position);
            if (dist > (extractionHelp.leaveRadius + 0.5f))
            {
                // outside of the circle?
                // cancel the leaving
                photonView.RPC("CancelLeave", RpcTarget.All);
                // if there are 2 players, signal the other
                if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                {
                    GetComponent<PlayerManager>().getOtherPlayer().GetPhotonView().RPC("CancelLeave", RpcTarget.All);
                }

            }
        }
    }

    [PunRPC]
    public void FirstPlayerReadyToLeave()
    {
        if (photonView.IsMine)
        {
            // the other player is ready to leave?
            extractionHelp.FirstPlayerReadyToLeave();
        }
    }

    [PunRPC]
    public void SecondPlayerReadyToLeave()
    {
        if (photonView.IsMine)
        {
            // the second player is ready to leave now
            extractionHelp.SecondPlayerReadyToLeave();
        }
    }

    [PunRPC]
    public void CancelLeave()
    {
        if (photonView.IsMine)
        {
            extractionHelp.CancelLeave();
        }
        
    }

    

    bool LookingAtTruck()
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
