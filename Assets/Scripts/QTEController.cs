using UnityEngine;
using UnityEngine.Events;

public class QTEController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform needle;
    [SerializeField] private RectTransform successZone;

    [Header("QTE Settings")]
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private KeyCode inputKey = KeyCode.Space;
    [SerializeField] private float attemptDuration = 2f;

    [Header("Success Zone")]
    [SerializeField] private float successHalfAngle = 20f;
    [SerializeField] private bool randomizeSuccessZoneOnStart = true;
    [SerializeField] private Vector2 successZoneAngleRange = new Vector2(-150f, 150f);

    public UnityEvent onSuccess = new UnityEvent();
    public UnityEvent onFail = new UnityEvent();

    private bool isActive;
    private float timer;

    void Update()
    {
        if (!isActive)
            return;

        if (needle != null)
            needle.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            EndQTE(false);
            return;
        }

        if (Input.GetKeyDown(inputKey))
            CheckSuccess();
    }

    public void StartQTE()
    {
        if (needle == null || successZone == null)
        {
            Debug.LogWarning("QTEController: faltan referencias de needle/successZone.");
            return;
        }

        gameObject.SetActive(true);
        isActive = true;
        timer = attemptDuration;

        if (randomizeSuccessZoneOnStart)
        {
            float targetAngle = Random.Range(successZoneAngleRange.x, successZoneAngleRange.y);
            successZone.localEulerAngles = new Vector3(0f, 0f, targetAngle);
        }

        needle.localEulerAngles = Vector3.zero;
    }

    private void CheckSuccess()
    {
        float needleAngle = NormalizeSignedAngle(needle.localEulerAngles.z);
        float zoneAngle = NormalizeSignedAngle(successZone.localEulerAngles.z);
        float delta = Mathf.Abs(Mathf.DeltaAngle(needleAngle, zoneAngle));

        bool success = delta <= successHalfAngle;

        EndQTE(success);
    }

    private void EndQTE(bool success)
    {
        isActive = false;
        gameObject.SetActive(false);

        if (success)
            onSuccess.Invoke();
        else
            onFail.Invoke();
    }

    private float NormalizeSignedAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f)
            angle -= 360f;

        return angle;
    }
}
