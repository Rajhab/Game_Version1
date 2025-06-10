using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private InputSystem_Actions playerControls;

    private float rotationX = 0f;
    private float rotationY = 0f;
    private Vector2 currentMoveInput;

    Rigidbody rb;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private float lookSensitivity = 0.5f;
    [SerializeField] private float walkSpeed = 2.5f;
    [SerializeField] private float sprintSpeed = 5.0f;

    [SerializeField] private float currentMoveSpeed;

    private bool isSprinting = false;

    private void Awake()
    {
        //Freezed rotation vom Rigidbody (Player) damit dieser nicht umkippt
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        //Locked Mauszeiger, damit der Spieler nicht aus dem Spiel herausklicken kann
        mainCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerControls = new InputSystem_Actions();

        playerControls.Player.Move.performed += OnMovePerformed;
        playerControls.Player.Move.canceled += OnMoveCanceled;

        playerControls.Player.Sprint.performed += OnSprintPerformed;
        playerControls.Player.Sprint.canceled += OnSprintCanceled;

        playerControls.Player.Look.performed += OnLookPerformed;
        playerControls.Player.Look.canceled += OnLookCanceled;

        currentMoveSpeed = walkSpeed;
    }

    private void Update()
    {
        if (isSprinting) 
        {
            currentMoveSpeed = sprintSpeed;
        }
        else
        {           
            currentMoveSpeed = walkSpeed;
        }

        //Movement basierend auf der Eingabe
        Vector3 moveDirection = (transform.forward * currentMoveInput.y + transform.right * currentMoveInput.x);
        transform.position += moveDirection * currentMoveSpeed * Time.deltaTime;
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