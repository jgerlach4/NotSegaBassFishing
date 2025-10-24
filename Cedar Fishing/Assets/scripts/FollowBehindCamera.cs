using UnityEngine;

public class FollowBehindCamera : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("The player transform to follow")]
    public Transform target;

    [Header("Camera Position")]
    [Tooltip("Distance behind the player")]
    public float distance = 5f;
    
    [Tooltip("Height above the player")]
    public float height = 2f;
    
    [Tooltip("How smoothly the camera follows (lower = smoother but more lag)")]
    public float smoothSpeed = 10f;

    [Header("Look At")]
    [Tooltip("Offset from player position to look at (useful for looking at chest/head instead of feet)")]
    public Vector3 lookAtOffset = new Vector3(0f, 1.5f, 0f);

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("FollowBehindCamera: No target assigned!");
            return;
        }

        // Calculate the desired position behind the player
        // Use the player's forward direction to stay behind them
        Vector3 desiredPosition = target.position - target.forward * distance + Vector3.up * height;

        // Smoothly move the camera towards the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Make the camera look at the player (with offset)
        Vector3 lookAtPoint = target.position + lookAtOffset;
        transform.LookAt(lookAtPoint);
    }

    // Visualize the camera position in the editor
    void OnDrawGizmosSelected()
    {
        if (target == null) return;

        // Draw the desired camera position
        Vector3 desiredPosition = target.position - target.forward * distance + Vector3.up * height;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(desiredPosition, 0.3f);

        // Draw line from camera to look-at point
        Vector3 lookAtPoint = target.position + lookAtOffset;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(desiredPosition, lookAtPoint);
        Gizmos.DrawWireSphere(lookAtPoint, 0.2f);
    }
}