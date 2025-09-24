using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Tooltip("The transform the camera should follow.")]
    public Transform target;

    [Tooltip("How quickly the camera moves to follow the target. 0 = instant snap.")]
    [Range(0f, 10f)]
    public float smoothSpeed = 5f;

    [Tooltip("Z-offset so the camera stays back (usually negative).")]
    public float zOffset = -10f;

    private void LateUpdate()
    {
        if (target == null) return;

        // Desired camera position: follow target in X/Y, keep fixed Z
        Vector3 desired = new Vector3(target.position.x, target.position.y, zOffset);

        if (smoothSpeed <= 0f)
        {
            // Instant snap
            transform.position = desired;
        }
        else
        {
            // Smooth follow
            transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Change what the camera follows at runtime.
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}