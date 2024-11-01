using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public bool canMove = true;
    public bool isSprinting = false;

    [Header("Move Parameters")]
    public float walkSpeed = 6f;
    public float sprintSpeed = 12f;
    public float jumpPower = 11f;
    public float gravity = 16f;

    [Header("Dash Parameters")]
    public float dashSpeed = 20f;           // Speed during the dash
    public float dashDuration = 0.2f;       // Duration of each dash
    public float dashCooldown = 1f;         // Cooldown before both dashes reset
    public int maxDashes = 2;               // Maximum dashes before cooldown

    [Header("Camera Parameters")]
    public Camera fpsCamera;
    public float lookSpeed = 2f;
    public float lookLimit = 45f;

    private CharacterController characterController;
    private Vector3 moveVelocity;
    private Vector2 moveInput;

    private Vector2 lookInput;
    private float rotation;

    [Header("Double Jump Parameters")]
    public LayerMask groundLayer;           // LayerMask to specify what counts as "ground"
    public Transform groundCheck;           // Transform below the player's feet (to cast the ray from)
    private readonly float groundCheckDistance = 0.3f;  // Distance the ray will check for the ground
    private readonly int maxJumps = 2;       // Maximum number of jumps (including double jump)
    private int jumpsLeft;                   // Tracks remaining jumps
    private bool isGrounded;                 // Whether the player is currently grounded
    private bool wasGrounded = false;       // Track the previous grounded state

    private bool isDashing = false;          // Is the player currently dashing
    private float dashTime;                  // Timer for the dash duration
    private Vector3 dashDirection;           // Direction of the dash
    private int dashesLeft;                  // Dashes remaining before cooldown
    private float dashCooldownTimer;         // Timer for dash cooldown

    public LayerMask trampolineLayer;
    private bool onTrampoline = false;
    public float trampolineVelocity = 15f;

    [Header("References")]
    public Transform orientation;
    public Rigidbody rb;
    public LayerMask whatIsWall;

    [Header("Climbing")]
    public float climbSpeed;
    public float maxClimbTime;
    private float climbTimer;

    private bool climbing;

    [Header("Climbing")]
    public float detectionLength;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront;

    private Vector3 climbNormal; // The normal of the wall the player is climbing

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        jumpsLeft = maxJumps;
        dashesLeft = maxDashes; // Initialize dashes left
        dashCooldownTimer = 0f;  // Initialize dash cooldown timer
    }

    // Update is called once per frame
    void Update()
    {
        // Player Movement
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float currentSpeedX = canMove ? (isSprinting ? sprintSpeed : walkSpeed) * moveInput.y : 0f;
        float currentSpeedY = canMove ? (isSprinting ? sprintSpeed : walkSpeed) * moveInput.x : 0f;

        float movementVelocity = moveVelocity.y;
        moveVelocity = (forward * currentSpeedX) + (right * currentSpeedY);

        // Jumping
        moveVelocity.y = onTrampoline ? trampolineVelocity : movementVelocity;

        if (climbing) moveVelocity.y = climbSpeed;

        if (!characterController.isGrounded)
        {
            moveVelocity.y -= gravity * Time.deltaTime;
        }

        // Handle dashing
        if (isDashing)
        {
            dashTime += Time.deltaTime;
            characterController.Move(dashSpeed * Time.deltaTime * dashDirection); // Apply dash movement
            if (dashTime >= dashDuration)
            {
                isDashing = false; // End the dash
            }
        }
        else
        {
            characterController.Move(moveVelocity * Time.deltaTime); // Regular movement
        }

        // Camera
        if (canMove)
        {
            rotation += -lookInput.y * lookSpeed;
            rotation = Mathf.Clamp(rotation, -lookLimit, lookLimit);

            fpsCamera.transform.localRotation = Quaternion.Euler(rotation, 0, 0);
            transform.rotation *= Quaternion.Euler(0, lookInput.x * lookSpeed, 0);
        }

        CheckGrounded();

        // Reset jumps only when the player has landed
        if (isGrounded && !wasGrounded)
        {
            jumpsLeft = maxJumps; // Reset jumps when grounded after being airborne
        }

        wasGrounded = isGrounded; // Update the previous grounded state

        CheckTrampoline();

        // Update the dash cooldown timer and reset dashes if needed
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer <= 0)
            {
                dashesLeft = maxDashes; // Reset dashes when cooldown ends
            }
        }

        WallCheck();
        StateMachine();

        if (climbing) ClimbingMovement();
    }

    public void HandleMoveInput(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void HandleSprintInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            isSprinting = true;
        }
        else if (ctx.canceled)
        {
            isSprinting = false;
        }
    }

    public void HandleJumpInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && canMove && jumpsLeft > 0)
        {
            moveVelocity.y = jumpPower;
            jumpsLeft--;
        }
    }

    public void HandleLookInput(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }

    public void HandleDashInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isDashing && dashesLeft > 0)
        {
            Dash();
        }
    }

    private void Dash()
    {
        // Set the dash direction based on current movement input
        dashDirection = (transform.forward).normalized; // Dash forward in the direction the player is facing
        isDashing = true;
        dashTime = 0f; // Reset the dash timer

        dashesLeft--; // Reduce the dash count
        if (dashesLeft <= 0)
        {
            dashCooldownTimer = dashCooldown; // Start the cooldown if no dashes are left
        }
    }

    void CheckTrampoline()
    {
        if (Physics.Raycast(groundCheck.position, Vector3.down, out RaycastHit hit, groundCheckDistance, trampolineLayer))
        {
            onTrampoline = true;
            jumpsLeft = maxJumps;
        }
        else
        {
            onTrampoline = false;
        }
    }

    void CheckGrounded()
    {
        if (Physics.Raycast(groundCheck.position, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void StateMachine()
    {
        if (wallFront && Input.GetKey(KeyCode.W) && wallLookAngle < maxWallLookAngle)
        {
            if (!climbing && climbTimer > 0) StartClimbing();

            if (climbTimer > 0) climbTimer -= Time.deltaTime;
            if (climbTimer < 0) StopClimbing();
        }
        else
        {
            if (climbing) StopClimbing();
        }
    }

    public void WallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, whatIsWall);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        // climbNormal = frontWallHit.normal;

        if (isGrounded)
        {
            climbTimer = maxClimbTime;
        }
    }

    private void StartClimbing()
    {
        climbing = true;
    }

    private void ClimbingMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);
        // Vector3 climbMovement = new Vector3(moveInput.x, moveInput.y, 0) * climbSpeed;
        // Vector3 wallMovement = Vector3.ProjectOnPlane(climbMovement, climbNormal);

        // characterController.Move(wallMovement * Time.deltaTime);
    }

    private void StopClimbing()
    {
        climbing = false;
    }
}
