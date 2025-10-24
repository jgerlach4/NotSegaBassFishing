using UnityEngine;

[AddComponentMenu("Camera/Third Person Follow (Behind)")]
public class ThirdPersonFollow : MonoBehaviour
{
    [Tooltip("The player or object the camera follows")]
    public Transform target;

    [Tooltip("How far behind the player (along player's -forward)")]
    public float distance = 4.0f;

    [Tooltip("Height above the player")]
    public float height = 1.8f;

    [Tooltip("How quickly the camera follows position")]
    public float positionSmoothTime = 0.12f;

    [Tooltip("Optional offset added to the look-at point (useful to look at head rather than feet)")]
    public Vector3 lookAtOffset = new Vector3(0f, 1.2f, 0f);

    [Header("Mouse Look Settings")]
    [Tooltip("Horizontal mouse sensitivity")]
    public float mouseSensitivityX = 100f;

    [Tooltip("Vertical mouse sensitivity")]
    public float mouseSensitivityY = 100f;

    [Tooltip("Minimum vertical angle (looking down)")]
    public float minVerticalAngle = -30f;

    [Tooltip("Maximum vertical angle (looking up)")]
    public float maxVerticalAngle = 60f;

    [Tooltip("Lock/unlock cursor on start")]
    public bool lockCursor = true;

    private float currentYaw = 0f;   // Horizontal rotation
    private float currentPitch = 0f; // Vertical rotation
    Vector3 currentVelocity;

    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Initialize rotation angles from target's current rotation
        if (target != null)
        {
            currentYaw = target.eulerAngles.y;
        }
    }

    void Update()
    {
        // Toggle cursor lock with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
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

        // Only process mouse look when cursor is locked
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            // Get raw mouse input (more responsive)
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Debug: Print mouse values to console
            if (mouseX != 0 || mouseY != 0)
            {
                Debug.Log($"Mouse X: {mouseX}, Mouse Y: {mouseY}");
            }

            // Update rotation angles
            currentYaw += mouseX * mouseSensitivityX * Time.deltaTime;
            currentPitch -= mouseY * mouseSensitivityY * Time.deltaTime;

            // Clamp vertical angle
            currentPitch = Mathf.Clamp(currentPitch, minVerticalAngle, maxVerticalAngle);
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate rotation based on mouse input
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);

        // Calculate desired position using the rotation
        Vector3 offset = rotation * new Vector3(0f, height, -distance);
        Vector3 desiredPos = target.position + offset;

        // Smooth the position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref currentVelocity, positionSmoothTime);

        // Look at the target
        Vector3 lookAtPoint = target.position + lookAtOffset;
        transform.LookAt(lookAtPoint);
    }

    // debug visual in editor
    void OnDrawGizmosSelected()
    {
        if (target == null) return;
        Gizmos.color = Color.cyan;
        
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        Vector3 offset = rotation * new Vector3(0f, height, -distance);
        Vector3 desiredPos = target.position + offset;
        
        Gizmos.DrawWireSphere(desiredPos, 0.15f);
        Gizmos.DrawLine(target.position + lookAtOffset, desiredPos);
    }
}