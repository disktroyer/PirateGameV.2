using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("A quién seguir")]
    public Transform target;

    [Header("Offset de cámara (editable)")]
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Suavidad")]
    public float smoothSpeed = 5f;
    private float currentSpeed;
    private float baseSpeed;

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        if (target != null)
        {
            transform.position = new Vector3(
                target.position.x + offset.x,
                target.position.y + offset.y,
                offset.z
            );
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // Enfocar temporalmente un objetivo sin cambiar el `target` permanente.
    private Transform focusTarget = null;
    private float focusTimer = 0f;

    public void FocusOn(Transform newFocus, float duration)
    {
        focusTarget = newFocus;
        focusTimer = duration;
    }

    void LateUpdate()
    {
        Transform activeTarget = target;
        if (focusTimer > 0f && focusTarget != null)
        {
            activeTarget = focusTarget;
            focusTimer -= Time.deltaTime;
        }

        if (activeTarget == null) return;

        Vector3 desiredPosition = activeTarget.position + new Vector3(offset.x, offset.y, offset.z);

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
