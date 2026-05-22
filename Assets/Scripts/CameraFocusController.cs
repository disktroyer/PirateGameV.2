using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFocusController : MonoBehaviour
{
    [Header("Target")]
    public Transform defaultTarget;

    [Header("Offset de cámara")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Suavidad")]
    public float smoothSpeed = 5f;

    private Transform focusTarget;
    private float focusTimer;

    void Start()
    {
        if (defaultTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                defaultTarget = player.transform;
        }
    }

    void LateUpdate()
    {
        Transform activeTarget = defaultTarget;

        if (focusTimer > 0f && focusTarget != null)
        {
            activeTarget = focusTarget;
            focusTimer -= Time.deltaTime;

            if (focusTimer <= 0f)
            {
                focusTarget = null;
            }
        }

        if (activeTarget == null)
            return;

        Vector3 desiredPosition = new Vector3(
            activeTarget.position.x,
            activeTarget.position.y,
            offset.z
        );

        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.position = smoothedPosition;
    }

    public void SetDefaultTarget(Transform newTarget)
    {
        defaultTarget = newTarget;
    }

    public void FocusOn(Transform newFocus, float duration)
    {
        focusTarget = newFocus;
        focusTimer = Mathf.Max(duration, 0.01f);
    }
}
