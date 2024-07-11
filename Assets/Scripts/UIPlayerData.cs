using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerData : MonoBehaviour
{
    [SerializeField] private Text speedText;
    [SerializeField] private Text mode;
    [SerializeField] private Text keyInput;

    PlayerMovement playerMovement;

    Rigidbody rb;
    float currentMoveSpeed;

    float standValue = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        CurrentSpeed();
        CurrentMoveMode();
        CurrentKeyInput();
    }

    private void CurrentSpeed()
    {
        // shows the current speed
        currentMoveSpeed = rb.velocity.magnitude;
        speedText.text = "Speed: " + currentMoveSpeed.ToString();
    }

    private void CurrentMoveMode()
    {
        if (currentMoveSpeed <= standValue && !playerMovement.isCrouching)
        {
            mode.text = "Mode: Standing";
        }
        else if (currentMoveSpeed <= standValue && playerMovement.isCrouching)
        {
            mode.text = "Mode: Crouching";
        }
        else if (currentMoveSpeed > standValue && playerMovement.isCrouching)
        {
            mode.text = "Mode: Crouch Walking";
        }
        else if (playerMovement.isSprinting)
        {
            mode.text = "Mode: Running";
        }
        else if (playerMovement.isJumping)
        {
            mode.text = "Mode: Jumping";
        }
        else if (playerMovement.isDodging)
        {
            mode.text = "Mode: Dodging";
        }
        else
            mode.text = "Mode: Walking";
    }

    private void CurrentKeyInput()
    {
        foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(vKey))
            {
                keyInput.text = "Key Input: " + vKey.ToString();
            }
        }
    }
}
