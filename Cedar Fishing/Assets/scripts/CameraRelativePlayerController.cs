using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class CameraRelativePlayerController : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera; // assign in inspector

    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float rotationSpeed = 10f;

    const string WALK_PARAM = "isWalking";
    const string RUN_PARAM = "isRunning";

    CharacterController controller;
    Animator animator;

    float verticalVelocity = 0f;
    public float gravity = 9.81f;
    public float groundedOffset = -0.1f;

    // debug toggle
    public bool verboseDebug = false;
    float debugLogInterval = 0.25f;
    float debugTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (controller == null) Debug.LogError("CharacterController missing!");
        if (animator == null) Debug.LogError("Animator missing!");
        if (playerCamera == null) Debug.LogError("playerCamera not assigned in inspector!");
    }

    void Update()
    {
        HandleMovement();

        // throttle debug log
        if (verboseDebug)
        {
            debugTimer -= Time.deltaTime;
            if (debugTimer <= 0f)
            {
                debugTimer = debugLogInterval;
                PrintInputDebug();
            }
        }
    }

    void HandleMovement()
    {
        if (Keyboard.current == null) return;

        // Read direct keys
        float keyH = 0f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) keyH = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) keyH = 1f;

        float keyV = 0f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) keyV = -1f;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) keyV = 1f;

        float h = keyH;
        float v = keyV;

        bool runPressed = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rKey.isPressed;

        // Build camera-relative movement (ignore camera pitch)
        Vector3 camForward = Vector3.forward;
        Vector3 camRight = Vector3.right;
        if (playerCamera != null)
        {
            camForward = playerCamera.transform.forward;
            camForward.y = 0f;
            camForward.Normalize();

            camRight = playerCamera.transform.right;
            camRight.y = 0f;
            camRight.Normalize();
        }
        else
        {
            // fallback to player transform
            camForward = transform.forward;
            camForward.y = 0f;
            camRight = transform.right;
            camRight.y = 0f;
        }

        Vector3 desiredMove = camForward * v + camRight * h;
        bool isMoving = desiredMove.sqrMagnitude > 0.0001f;

        // Rotate player to face movement direction
        if (isMoving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(desiredMove);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Animator
        animator.SetBool(WALK_PARAM, isMoving);
        animator.SetBool(RUN_PARAM, isMoving && runPressed);

        float speed = (isMoving && runPressed) ? runSpeed : walkSpeed;
        Vector3 horizontalVelocity = (isMoving ? desiredMove.normalized * speed : Vector3.zero);

        // gravity & grounded
        if (controller.isGrounded)
        {
            verticalVelocity = groundedOffset;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        Vector3 finalVel = horizontalVelocity + Vector3.up * verticalVelocity;
        controller.Move(finalVel * Time.deltaTime);
    }

    void PrintInputDebug()
    {
        if (Keyboard.current == null)
        {
            Debug.Log("[Input Debug] Keyboard.current is NULL");
            return;
        }

        // Show key states
        bool w = Keyboard.current.wKey.isPressed;
        bool a = Keyboard.current.aKey.isPressed;
        bool s = Keyboard.current.sKey.isPressed;
        bool d = Keyboard.current.dKey.isPressed;
        bool up = Keyboard.current.upArrowKey.isPressed;
        bool left = Keyboard.current.leftArrowKey.isPressed;
        bool down = Keyboard.current.downArrowKey.isPressed;
        bool right = Keyboard.current.rightArrowKey.isPressed;
        bool leftShift = Keyboard.current.leftShiftKey.isPressed;

        Vector2 mouseDelta = Mouse.current != null ? Mouse.current.delta.ReadValue() : Vector2.zero;

        Debug.Log($"[Input Debug] Keys: W:{w} A:{a} S:{s} D:{d}  Arrows U:{up} L:{left} D:{down} R:{right}  Shift:{leftShift}  Mouse Delta:{mouseDelta}");
    }
}
