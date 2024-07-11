using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;

    [SerializeField] private float groundDrag;

    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    public bool isJumping {  get; private set; }

    [SerializeField] private float crouchMoveSpeed;
    public bool isCrouching { get; private set; }

    [SerializeField] private float sprintSpeed;
    [SerializeField] private float holdTimeForSprint;
    public bool isSprinting { get; private set; }

    [SerializeField] private float dodgeForce;
    [SerializeField] private float dodgeCooldown;
    public bool isDodging { get; private set; }

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.C;
    [SerializeField] private KeyCode sprintDodgeKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    bool grounded;

    [SerializeField] private Transform orientation;

    // Cache variables for values
    float horizontalInput;
    float verticalInput;

    float currentMoveSpeed;
    float startTime = 0f;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        currentMoveSpeed = moveSpeed;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        isJumping = false;
        isCrouching = false;
        isSprinting = false;
        isDodging = false;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if(Input.GetKeyDown(jumpKey) && !isJumping && grounded)
        {
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // when to crouch
        if(Input.GetKeyDown(crouchKey) && !isCrouching && grounded)
        {
            Crouch();

        }else if (Input.GetKeyDown(crouchKey) && isCrouching && grounded)
        {
            ResetCrouch();
        }

        // check for sprint or dodge
        if (Input.GetKeyDown(sprintDodgeKey))
        {
            isDodging = true;
            startTime = Time.time;
        }

        // when to sprint
        if (Input.GetKey(sprintDodgeKey) && !isSprinting && grounded && startTime + holdTimeForSprint < Time.time) 
        {
            isDodging = false;
            Sprint();
        }
        else if (Input.GetKeyUp(sprintDodgeKey) && isSprinting && grounded)
        {
            startTime = 0;
            ResetSprint();
        }

        // when to dodge
        if (Input.GetKeyUp(sprintDodgeKey) && isDodging && grounded)
        {
            Dodge();
            Invoke(nameof(ResetDodge), dodgeCooldown);
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * currentMoveSpeed * 10.0f, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > currentMoveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * currentMoveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        isJumping = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        // reset crouch by jump
        CancelCrouch();
    }

    private void ResetJump()
    {
        isJumping = false;
    }

    private void Crouch()
    {
        isCrouching = true;

        // simulates the height of the player
        gameObject.transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z);

        // make the player move slower
        currentMoveSpeed = crouchMoveSpeed;
    }

    private void ResetCrouch()
    {
        isCrouching = false;

        // reset the height to his normal
        gameObject.transform.localScale = new Vector3(transform.localScale.x, 1.0f, transform.localScale.z);

        // return his normal move speed
        currentMoveSpeed = moveSpeed;
    }

    private void CancelCrouch()
    {
        if (isCrouching)
            ResetCrouch();
    }

    private void Sprint()
    {
        isSprinting = true;

        currentMoveSpeed = sprintSpeed;

        // reset crouch by sprint
        CancelCrouch();
    }

    private void ResetSprint()
    {
        isSprinting = false;
        
        currentMoveSpeed = moveSpeed;
    }

    private void Dodge()
    {
        isDodging = true;

        rb.AddForce(moveDirection * dodgeForce, ForceMode.Impulse);

        // reset crouch by dodge
        CancelCrouch();
    }

    private void ResetDodge()
    {
        isDodging = false;
    }
}
