using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("A quién seguir")]
    public Transform target;

    [Header("Offset de cámara (editable)")]
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Suavidad")]
    public float smoothSpeed = 5f;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = new Vector3(
    target.position.x,
    target.position.y,
    offset.z
);

        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.position = smoothedPosition;
    }

    public void SetSpeedMultiplier(float multiplier)
{
    currentSpeed = baseSpeed * multiplier;
}

public void ResetSpeed()
{
    currentSpeed = baseSpeed;
}

    
}
