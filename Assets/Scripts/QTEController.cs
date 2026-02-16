using UnityEngine;
using UnityEngine.Events;

public class QTEController : MonoBehaviour
{
    [Header("UI")]
    public RectTransform needle;
    public RectTransform successZone;

    [Header("Config")]
    public float rotationSpeed = 200f;
    public float successTolerance = 25f;
    public KeyCode inputKey = KeyCode.E;

    [Header("Optional Critical Zone")]
    public bool useCriticalZone = false;
    public RectTransform criticalZone;
    public float criticalTolerance = 10f;

    [Header("Events")]
    public UnityEvent onSuccess;
    public UnityEvent onFail;
    public UnityEvent onCriticalFail;

    private bool isActive = false;

    void Update()
    {
        if (!isActive) return;

        needle.Rotate(0, 0, -rotationSpeed * Time.deltaTime);

        if (Input.GetKeyDown(inputKey))
        {
            CheckResult();
        }
    }

    public void StartQTE()
    {
        needle.localRotation = Quaternion.identity;
        isActive = true;
        gameObject.SetActive(true);
    }

    public void StopQTE()
    {
        isActive = false;
        gameObject.SetActive(false);
    }

    void CheckResult()
    {
        float needleZ = needle.eulerAngles.z;
        float zoneZ = successZone.eulerAngles.z;

        float difference = Mathf.Abs(Mathf.DeltaAngle(needleZ, zoneZ));

        if (useCriticalZone && criticalZone != null)
        {
            float criticalZ = criticalZone.eulerAngles.z;
            float criticalDiff = Mathf.Abs(Mathf.DeltaAngle(needleZ, criticalZ));

            if (criticalDiff < criticalTolerance)
            {
                onCriticalFail?.Invoke();
                StopQTE();
                return;
            }
        }

        if (difference < successTolerance)
        {
            onSuccess?.Invoke();
        }
        else
        {
            onFail?.Invoke();
        }

        StopQTE();
    }
}
