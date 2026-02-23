using UnityEngine;
using UnityEngine.UI;
using System;

public class QTEWheelControllerUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform needle;        // Aguja
    [SerializeField] private RectTransform successZone;   // Zona verde

    [Header("QTE Settings")]
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private KeyCode inputKey = KeyCode.Space;

    private bool isActive = false;
    private Action<bool> onFinish;

    void Update()
    {
        if (!isActive) return;

        // Rotar aguja
        needle.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // Input jugador
        if (Input.GetKeyDown(inputKey))
        {
            CheckSuccess();
        }
    }

    public void StartQTE(Action<bool> callback)
    {
        gameObject.SetActive(true);
        isActive = true;
        onFinish = callback;
    }

    private void CheckSuccess()
    {
        float angle = Mathf.Abs(
            Mathf.DeltaAngle(
                needle.eulerAngles.z,
                successZone.eulerAngles.z
            )
        );

        bool success = angle < 20f;

        EndQTE(success);
    }

    private void EndQTE(bool success)
    {
        isActive = false;
        gameObject.SetActive(false);
        onFinish?.Invoke(success);
    }
}
