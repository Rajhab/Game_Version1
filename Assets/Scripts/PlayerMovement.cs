using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private InputSystem_Actions playerControls;
    private Rigidbody rb;
    private CapsuleCollider playerCollider;

    private float rotationX = 0f;
    private float rotationY = 0f;
    private Vector2 currentMoveInput;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private float lookSensitivity = 0.5f;
    [SerializeField] private float walkSpeed = 2.5f;
    [SerializeField] private float sprintSpeed = 5.0f;
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;

    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;
    [SerializeField] private float fallMultiplier = 2.5f;

    [SerializeField] private float standHeight = 1.8f;
    [SerializeField] private float crouchHeight = 0.9f;
    [SerializeField] private float cameraStandHeightOffset = 0.8f;
    [SerializeField] private float cameraCrouchHeightOffset = 0.3f;
    [SerializeField] private float crouchSmoothTime = 0.1f;
    private bool isCrouchingTarget = false;
    // This variable was used for all smoothDamp calls, but will now be replaced by specific ones below.
    // private float currentCrouchVelocity; // User's original line, now effectively superseded by new ones

    // Separate velocities for smoothDamp for collider height, center, and camera to prevent interference
    private float currentHeightVelocity;
    private float currentCenterYVelocity;
    private float currentCameraYVelocity;

    [SerializeField] private float currentMoveSpeed;

    private bool isSprinting = false;

    private void Awake()
    {
        //Freezed rotation vom Rigidbody (Player) damit dieser nicht umkippt
        rb = GetComponent<Rigidbody>();
        if (rb == null) // Ensure Rigidbody is present and configured
        {
            Debug.LogError("Rigidbody component not found on this GameObject. Jumping and proper physics require a Rigidbody. Adding one now.");
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.freezeRotation = true;

        //Locked Mauszeiger, damit der Spieler nicht aus dem Spiel herausklicken kann
        mainCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerControls = new InputSystem_Actions();
        playerCollider = GetComponent<CapsuleCollider>();
        if (playerCollider == null) // Ensure CapsuleCollider is present and configured
        {
            Debug.LogError("CapsuleCollider component not found on this GameObject. Crouching requires a CapsuleCollider. Adding one now.");
            playerCollider = gameObject.AddComponent<CapsuleCollider>();
        }
        // Initialize collider with stand height to match initial state
        playerCollider.height = standHeight;
        playerCollider.center = new Vector3(playerCollider.center.x, standHeight / 2f, playerCollider.center.z); // Added 'f'
        playerCollider.radius = 0.4f; // A sensible default radius

        playerControls.Player.Move.performed += OnMovePerformed;
        playerControls.Player.Move.canceled += OnMoveCanceled;

        playerControls.Player.Sprint.performed += OnSprintPerformed;
        playerControls.Player.Sprint.canceled += OnSprintCanceled;

        playerControls.Player.Jump.performed += OnJumpPerformed;

        playerControls.Player.Crouch.performed += OnCrouchPerformed;
        playerControls.Player.Crouch.canceled += OnCrouchCanceled;

        playerControls.Player.Look.performed += OnLookPerformed;
        playerControls.Player.Look.canceled += OnLookCanceled;

        currentMoveSpeed = walkSpeed;
    }

    private void Update()
    {
        float targetSpeed = walkSpeed;
        if (isSprinting)
        {
            targetSpeed = sprintSpeed;
        }
        if (isCrouchingTarget)
        {
            targetSpeed *= crouchSpeedMultiplier;
        }

        currentMoveSpeed = targetSpeed;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        //Movement basierend auf der Eingabe
        Vector3 moveDirection = (transform.forward * currentMoveInput.y + transform.right * currentMoveInput.x);
        transform.position += moveDirection * currentMoveSpeed * Time.deltaTime;

        float targetColliderHeight = isCrouchingTarget ? crouchHeight : standHeight;
        float targetColliderCenterY = isCrouchingTarget ? crouchHeight / 2f : standHeight / 2f; // Use 'f' for float literal

        if (playerCollider != null)
        {
            // Smoothly adjust collider height using its own velocity variable
            playerCollider.height = Mathf.SmoothDamp(playerCollider.height, targetColliderHeight, ref currentHeightVelocity, crouchSmoothTime);
            // Smoothly adjust collider center Y using its own velocity variable
            playerCollider.center = new Vector3(playerCollider.center.x, Mathf.SmoothDamp(playerCollider.center.y, targetColliderCenterY, ref currentCenterYVelocity, crouchSmoothTime), playerCollider.center.z);
        }

        float targetCameraY = isCrouchingTarget ? cameraCrouchHeightOffset : cameraStandHeightOffset;
        Vector3 currentCameraLocalPos = mainCamera.transform.localPosition;
        // Smoothly adjust camera local Y position using its own velocity variable
        currentCameraLocalPos.y = Mathf.SmoothDamp(currentCameraLocalPos.y, targetCameraY, ref currentCameraYVelocity, crouchSmoothTime);
        mainCamera.transform.localPosition = currentCameraLocalPos;
    }

    // FixedUpdate is called at a fixed framerate, ideal for physics calculations.
    private void FixedUpdate()
    {
        // Apply extra gravity when falling
        if (rb.linearVelocity.y < 0) // If the player is moving downwards
        {
            // Add force multiplied by the fallMultiplier, subtracting 1 because default gravity is already applied
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        currentMoveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        currentMoveInput = Vector2.zero;
    }

    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        isSprinting = true;
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        isSprinting = false;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnCrouchPerformed(InputAction.CallbackContext context)
    {
        isCrouchingTarget = true;
    }

    private void OnCrouchCanceled(InputAction.CallbackContext context)
    {
        isCrouchingTarget = false;
    }

    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        //Rotation basierend auf der Eingabe
        Vector2 lookInput = context.ReadValue<Vector2>();
        rotationY += lookInput.x * lookSensitivity;
        rotationX -= lookInput.y * lookSensitivity;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        //Startposition der Kamera und Rotation
        if (mainCamera != null)
        {
            transform.rotation = Quaternion.Euler(0f, rotationY, 0f);

            mainCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        }
    }

    private void OnLookCanceled(InputAction.CallbackContext context)
    {

    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
}