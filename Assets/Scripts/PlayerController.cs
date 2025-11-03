using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    public float crouchHeight = 1f;

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;

    private CharacterController controller;
    private Camera playerCamera;
    private float verticalRotation = 0f;
    private Vector3 velocity;
    private bool isCrouching = false;
    private float originalHeight;
    // Hidden state used by hiding spots
    public bool isHidden { get; private set; } = false;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        originalHeight = controller.height;
        
        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!GameManager.Instance.isGameActive) return;

        // If player is hidden, skip movement and looking controls
        if (isHidden) return;

        HandleMovement();
        HandleMouseLook();
        HandleCrouch();
    }

    /// <summary>
    /// Sets the hidden state for the player. Hidden players won't process movement/look input.
    /// Use this when entering/exiting hiding spots instead of disabling the component.
    /// </summary>
    public void SetHidden(bool hidden)
    {
        isHidden = hidden;
        // When hidden, lock cursor so camera doesn't move unexpectedly
        if (isHidden)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        Vector3 move = transform.right * x + transform.forward * z;
        
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Handle gravity
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);

        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;
            controller.height = isCrouching ? crouchHeight : originalHeight;
        }
    }
}