using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Example : MonoBehaviour
{
    // These variables are to hold the Action references
    InputAction moveAction;
    InputAction jumpAction;
    InputAction lookAction;

    private Rigidbody rb;

    private float movementX;
    private float movementY;
    private float movementZ;

    private float lookX;
    private float lookY;
    private float lookZ;

    private bool isGrounded = true;

    public float speed = 10;

    // Camera Rotation
    public float mouseSensitivity = 0.2f;
    private Transform cameraTransform;

    private void Start()
    {
        // Find the references to the "Move" and "Jump" actions
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        lookAction = InputSystem.actions.FindAction("Look");

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        cameraTransform = Camera.main.transform;

        // Hides the mouse
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    void Update()
    {
        // Read the "Move" action value, which is a 2D vector
        // and the "Jump" action state, which is a boolean value

        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        // your movement code here

        movementX = moveValue.x;
        movementY = moveValue.y;

        RotateCamera();
    }

    void FixedUpdate()
    {

        Vector3 movement = (transform.right * movementX + transform.forward * movementY).normalized;
        Vector3 targetVelocity = movement * speed;

        // Apply movement to the Rigidbody
        Vector3 velocity = rb.linearVelocity;
        velocity.x = targetVelocity.x;
        velocity.z = targetVelocity.z;

        //Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.linearVelocity = velocity;
        
    }

    void RotateCamera()
    {
        Vector2 lookValue = lookAction.ReadValue<Vector2>();

        lookX = lookValue.x * mouseSensitivity;
        lookY -= lookValue.y * mouseSensitivity;

        transform.Rotate(0, lookX, 0);
        lookY = Mathf.Clamp(lookY, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(lookY, 0, 0);
    }

}