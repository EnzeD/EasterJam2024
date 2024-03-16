using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothing = 5f; // How smoothly the camera catches up with its target movement. The higher, the smoother.
    public float anticipationDistance = 2f; // Distance the camera moves ahead in the direction of the target's movement.

    private Vector3 offset; // The initial offset from the target.
    private Vector3 lastPosition; // Last frame's position of the target.

    void Start()
    {
        // Calculate the initial offset.
        offset = transform.position - target.position;
        lastPosition = target.position;
    }

    void FixedUpdate()
    {
        // Calculate the direction of movement.
        Vector3 direction = (target.position - lastPosition).normalized;
        // Anticipate the target's movement by adding anticipation distance in the direction of movement.
        Vector3 anticipatedPosition = target.position + (direction * anticipationDistance);
        // Create a position for the camera to move towards, incorporating the anticipation.
        Vector3 targetCamPos = anticipatedPosition + offset;
        // Smoothly interpolate between the camera's current position and its target position.
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
        // Update lastPosition for the next frame.
        lastPosition = target.position;
    }
}