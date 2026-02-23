using UnityEngine;

public class QTEWheelUI : MonoBehaviour
{
    [Header("References")]
    public RectTransform needle;
    public RectTransform successZone;

    [Header("Settings")]
    public float rotationSpeed = 200f;
    public float tolerance = 25f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip failSound;
    public AudioClip successSound;

    private PlayerQTEController controller;
    private bool active;

    void Update()
    {
        if (!active) return;

        needle.Rotate(0, 0, -rotationSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckSuccess();
        }
    }

    public void Show(PlayerQTEController ctrl)
    {
        controller = ctrl;
        needle.localRotation = Quaternion.identity;
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
        float difference = Mathf.Abs(
            Mathf.DeltaAngle(
                needle.eulerAngles.z,
                successZone.eulerAngles.z
            )
        );

        if (difference < tolerance)
        {
            if (successSound != null)
                audioSource.PlayOneShot(successSound);

            controller.OnSuccess();
        }
        else
        {
            if (failSound != null)
                audioSource.PlayOneShot(failSound);

            controller.OnFail(); // NO cerramos la rueda
        }
    }
}
