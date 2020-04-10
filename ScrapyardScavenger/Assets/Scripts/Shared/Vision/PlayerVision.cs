using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerVision : MonoBehaviourPunCallbacks
{
    public Transform player;
    public Transform cams;
    public Transform weapon;
    public Transform grenade;
    public Death playerDeath;

    public float xSensitivity;
    public float ySensitivity;
    public float maxAngle;

    private Quaternion camCenter;

    private bool isPaused;

    void Start()
    {
        camCenter = cams.localRotation;
        isPaused = false;

        // playerDeath = GetComponent<Death>();
        // playerDeath.OnPlayerDeath += OnDeath;
    }

    

    void Update()
    {
        // If not me, don't update!
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
        }

        UpdateCursorLock();
        if (!isPaused)
        {
            AdjustX();
            AdjustY();
        }
        
    }

    void OnApplicationFocus(bool hasFocus)
    {
        // If not me, don't update!
        if (!photonView.IsMine) return;


        isPaused = !hasFocus;
    }

    void AdjustY()
    {
        float inputY = Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;
        Quaternion adjustment = Quaternion.AngleAxis(inputY, Vector3.left);
        Quaternion delta = cams.localRotation * adjustment;

        if (Quaternion.Angle(camCenter, delta) < maxAngle)
        {
            cams.localRotation = delta;
        }

        weapon.rotation = cams.rotation;
        grenade.rotation = cams.rotation;
    }

    void AdjustX()
    {
        float inputX = Input.GetAxis("Mouse X") * xSensitivity * Time.deltaTime;
        Quaternion adjustment = Quaternion.AngleAxis(inputX, Vector3.up);
        Quaternion delta = player.localRotation * adjustment;
        player.localRotation = delta;
    }

    void UpdateCursorLock()
    {
        if (!isPaused)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void OnDeath(GameObject deadPlayer)
    {
        isPaused = true;
    }
}
