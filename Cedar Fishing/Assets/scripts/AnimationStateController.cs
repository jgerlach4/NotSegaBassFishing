/*using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationStateController : MonoBehaviour
{
    Animator animator;
    int isWalkingHash = Animator.StringToHash("isWalking");
    int isRunningHash = Animator.StringToHash("isRunning");
    int isCastingHash = Animator.StringToHash("isCasting");

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();    
    }

    // Update is called once per frame
    void Update()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool forwardPressed = Keyboard.current != null && Keyboard.current.wKey.isPressed;

        bool runPressed = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
        bool isRunning = animator.GetBool(isRunningHash);

        bool castingPressed = Keyboard.current != null && Keyboard.current.eKey.isPressed;
        bool isCasting = animator.GetBool(isCastingHash);

        //if we're not walking and w is pressed
        if (!isWalking && forwardPressed)
        {
            animator.SetBool(isWalkingHash, true);
        }

        //if we're walking and w is not pressed
        if (isWalking && (!forwardPressed))
        {
            animator.SetBool(isWalkingHash, false);
        }

        //if we're not running and both w and run key are pressed
        if ((forwardPressed && runPressed) && !isRunning)
        {
            animator.SetBool(isRunningHash, true);
        }

        //if we're running and either run key or w are not pressed
        if ((!runPressed || !forwardPressed) && isRunning)
        {
            animator.SetBool(isRunningHash, false);
        }

        if ((!forwardPressed && !isRunning) && !isCasting && castingPressed)
        {
<<<<<<< Updated upstream
            animator.SetBool("isCasting", true);
=======
            animator.SetBool(isCastingHash, true); 
>>>>>>> Stashed changes
        }

        if (isCasting && !castingPressed)
        {
            animator.SetBool(isCastingHash, false);
        }
    }
}*/

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class CameraRelativePlayerControllerDebug : MonoBehaviour
{
    int isCastingHash = Animator.StringToHash("isCasting");
    int isRunningHash = Animator.StringToHash("isRunning");
    
    [Header("References")]
    public Camera playerCamera; // assign in inspector

    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2.0f;
    public float maxPitch = 85f;
    public bool invertY = false;

    const string WALK_PARAM = "isWalking";
    const string RUN_PARAM  = "isRunning";

    CharacterController controller;
    Animator animator;

    float pitch = 0f;
    float verticalVelocity = 0f;
    public float gravity = 9.81f;
    public float groundedOffset = -0.1f;

    // debug toggle
    public bool verboseDebug = true;
    float debugLogInterval = 0.25f;
    float debugTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (controller == null) Debug.LogError("CharacterController missing!");
        if (animator == null) Debug.LogError("Animator missing!");
        if (playerCamera == null) Debug.LogError("playerCamera not assigned in inspector!");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        bool isCasting = animator.GetBool(isCastingHash);
        bool castingPressed = Keyboard.current != null && Keyboard.current.eKey.isPressed;
        bool isRunning = animator.GetBool(isRunningHash);
        bool forwardPressed = Keyboard.current != null && Keyboard.current.wKey.isPressed;

        if ((!forwardPressed && !isRunning) && !isCasting && castingPressed)
        {
            animator.SetBool(isCastingHash, true); 
        }

        if (isCasting && !castingPressed)
        {
            animator.SetBool(isCastingHash, false);
        }

        // quick guards
        if (Time.timeScale == 0f)
        {
            if (verboseDebug && debugTimer <= 0f) { Debug.LogWarning("Time.timeScale == 0, inputs won't have visible movement."); debugTimer = debugLogInterval; }
            debugTimer -= Time.deltaTime;
            return;
        }

        // Lock cursor on click
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            HandleMouseLook();
            HandleMovement();
        }

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

    void HandleMouseLook()
    {
        if (Mouse.current == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * mouseSensitivity * 0.02f; // Scale factor for new input system
        float mouseY = mouseDelta.y * mouseSensitivity * 0.02f;

        // yaw
        transform.Rotate(Vector3.up, mouseX);

        // pitch
        float invert = invertY ? 1f : -1f;
        pitch += mouseY * invert;
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);
        if (playerCamera != null)
            playerCamera.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);
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