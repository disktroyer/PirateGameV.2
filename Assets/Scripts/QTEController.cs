using UnityEngine;
using UnityEngine.Events;

public class QTEController : MonoBehaviour
{
    
    [Header("UI")]
    public RectTransform needle;
    public RectTransform successZone;

    [Header("Config")]
    public float rotationSpeed = 200f;
    public KeyCode inputKey = KeyCode.E;

    [Header("Events")]
    public UnityEvent onSuccess;
    public UnityEvent onFail;

    private bool isActive = false;

    void Update()
    {
        if (!isActive) return;

        // Girar aguja
        needle.Rotate(0, 0, -rotationSpeed * Time.deltaTime);

        // Input
        if (Input.GetKeyDown(inputKey))
        {
            CheckResult();
        }
    }

    public void StartQTE()
    {
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
        float needleZ = Mathf.Abs(needle.eulerAngles.z);
        float zoneZ = Mathf.Abs(successZone.eulerAngles.z);
        float tolerance = 25f;

        if (Mathf.Abs(needleZ - zoneZ) < tolerance)
        {
            onSuccess.Invoke();
        }
        else
        {
            onFail.Invoke();
        }

        StopQTE();
    }
}

