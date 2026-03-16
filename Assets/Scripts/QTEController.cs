using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class QTEController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform needle;        // Aguja
    [SerializeField] private RectTransform successZone;   // Zona verde

    [Header("QTE Settings")]
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private KeyCode inputKey = KeyCode.Space;

    public UnityEvent onSuccess = new UnityEvent();
    public UnityEvent onFail = new UnityEvent();

    private bool isActive = false;

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

    public void StartQTE()
    {
        gameObject.SetActive(true);
        isActive = true;
    }

    private void CheckSuccess()
    {
        float angle = Mathf.Abs(
            Mathf.DeltaAngle(
                needle.eulerAngles.z,
                successZone.eulerAngles.z
            )
        );

        Debug.Log($"Needle angle: {needle.eulerAngles.z}, Success angle: {successZone.eulerAngles.z}, Delta: {angle}");

        bool success = angle < 45f;  // Aumentado de 20f a 45f para más tolerancia

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
}
