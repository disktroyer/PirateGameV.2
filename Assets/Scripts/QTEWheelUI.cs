using UnityEngine;

public class QTEWheelUI : MonoBehaviour
{
    public RectTransform needle;
    public RectTransform successZone;
    public float rotationSpeed = 200f;
    public float successHalfAngle = 20f;
    public KeyCode inputKey = KeyCode.E;

    private PlayerQTEController controller;
    private bool active;

    void Update()
    {
        if (!active) return;

        if (needle != null)
            needle.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        if (Input.GetKeyDown(inputKey))
            CheckSuccess();
    }

    public void Show(PlayerQTEController ctrl)
    {
        controller = ctrl;
        active = true;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        active = false;
        gameObject.SetActive(false);
    }

    void CheckSuccess()
    {
        if (controller == null || needle == null)
            return;

        float needleAngle = NormalizeSignedAngle(needle.localEulerAngles.z);

        float zoneAngle = 0f;
        if (successZone != null)
            zoneAngle = NormalizeSignedAngle(successZone.localEulerAngles.z);

        float delta = Mathf.Abs(Mathf.DeltaAngle(needleAngle, zoneAngle));

        if (delta <= successHalfAngle)
            controller.OnSuccess();
        else
            controller.OnFail();
    }

    float NormalizeSignedAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f)
            angle -= 360f;

        return angle;
    }
}
