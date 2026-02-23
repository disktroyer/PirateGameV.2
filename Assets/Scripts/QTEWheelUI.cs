using UnityEngine;

public class QTEWheelUI : MonoBehaviour
{
    public RectTransform needle;
    public float rotationSpeed = 200f;
    public float successMin = -20f;
    public float successMax = 20f;

    private PlayerQTEController controller;
    private bool active;

    void Update()
    {
        if (!active) return;

        needle.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.E))
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
        float angle = needle.localEulerAngles.z;
        angle = angle > 180 ? angle - 360 : angle;

        if (angle >= successMin && angle <= successMax)
            controller.OnSuccess();
        else
            controller.OnFail();
    }
}
