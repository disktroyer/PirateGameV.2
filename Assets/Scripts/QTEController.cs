using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QTEController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform needle;
    [SerializeField] private RectTransform successZone;
    [SerializeField] private RectTransform qteBarArea;
    [SerializeField] private Canvas parentCanvas;

    [Header("Vertical Bar Settings")]
    [SerializeField] private float movementMinY = -120f;
    [SerializeField] private float movementMaxY = 120f;
    [SerializeField] private float indicatorSpeed = 260f;
    [SerializeField] private KeyCode inputKey = KeyCode.Space;
    [SerializeField] private float roundDuration = 2f;
    [SerializeField] private bool useQteBarAreaBounds = true;

    [Header("Rounds")]
    [SerializeField] private int requiredSuccesses = 3;
    [SerializeField] private int maxFails = 3;

    [Header("Success Zone")]
    [SerializeField] private bool randomizeSuccessZoneEachRound = true;
    [SerializeField] private Vector2 successZoneYRange = new Vector2(-100f, 100f);

    [Header("UI Indicators (3 recommended)")]
    [SerializeField] private List<GameObject> successIndicators = new List<GameObject>();
    [SerializeField] private List<GameObject> failIndicators = new List<GameObject>();

    public UnityEvent onSuccess = new UnityEvent();
    public UnityEvent onFail = new UnityEvent();

    private bool isActive;
    private bool movingUp = true;
    private float timer;
    private int successCount;
    private int failCount;

    void Awake()
    {
        if (parentCanvas == null)
            parentCanvas = GetComponentInParent<Canvas>();

        SetIndicatorState(successIndicators, 0);
        SetIndicatorState(failIndicators, 0);
    }

    void Update()
    {
        if (!isActive)
            return;

        MoveIndicator();

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            RegisterFail();
            return;
        }

        if (Input.GetKeyDown(inputKey))
            CheckHit();
    }

    public void StartQTE()
    {
        if (needle == null || successZone == null)
        {
            Debug.LogWarning("QTEController: faltan referencias de indicator/successZone.");
            return;
        }

        gameObject.SetActive(true);
        isActive = true;

        successCount = 0;
        failCount = 0;
        movingUp = true;

        SetIndicatorState(successIndicators, successCount);
        SetIndicatorState(failIndicators, failCount);

        GetNeedleBounds(out float minY, out _);
        SetNeedleY(minY);
        StartRound();
    }

    private void StartRound()
    {
        timer = roundDuration;

        if (randomizeSuccessZoneEachRound)
            RandomizeSuccessZoneY();
    }

    private void MoveIndicator()
    {
        if (needle == null)
            return;

        GetNeedleBounds(out float minY, out float maxY);

        float direction = movingUp ? 1f : -1f;
        float nextY = needle.anchoredPosition.y + (direction * indicatorSpeed * Time.deltaTime);

        if (nextY >= maxY)
        {
            nextY = maxY;
            movingUp = false;
        }
        else if (nextY <= minY)
        {
            nextY = minY;
            movingUp = true;
        }

        SetNeedleY(nextY);
    }

    private void CheckHit()
    {
        bool success = IsNeedleInsideSuccessZone();

        if (success)
            RegisterSuccess();
        else
            RegisterFail();
    }

    private bool IsNeedleInsideSuccessZone()
    {
        if (needle == null || successZone == null)
            return false;

        Camera uiCamera = GetUICamera();
        Vector2 needleScreenPoint = RectTransformUtility.WorldToScreenPoint(uiCamera, needle.TransformPoint(needle.rect.center));

        return RectTransformUtility.RectangleContainsScreenPoint(successZone, needleScreenPoint, uiCamera);
    }

    private void RegisterSuccess()
    {
        successCount++;
        SetIndicatorState(successIndicators, successCount);

        if (successCount >= Mathf.Max(1, requiredSuccesses))
        {
            EndQTE(true);
            return;
        }

        StartRound();
    }

    private void RegisterFail()
    {
        failCount++;
        SetIndicatorState(failIndicators, failCount);

        if (failCount >= Mathf.Max(1, maxFails))
        {
            EndQTE(false);
            return;
        }

        StartRound();
    }

    private void RandomizeSuccessZoneY()
    {
        if (successZone == null)
            return;

        GetSuccessZoneBounds(out float minY, out float maxY);

        if (minY > maxY)
        {
            float center = (minY + maxY) * 0.5f;
            SetSuccessZoneY(center);
            return;
        }

        float randomY = Random.Range(minY, maxY);
        SetSuccessZoneY(randomY);
    }

    private void SetNeedleY(float value)
    {
        if (needle == null)
            return;

        Vector2 pos = needle.anchoredPosition;
        pos.y = value;
        needle.anchoredPosition = pos;
    }

    private void SetSuccessZoneY(float value)
    {
        if (successZone == null)
            return;

        Vector2 pos = successZone.anchoredPosition;
        pos.y = value;
        successZone.anchoredPosition = pos;
    }

    private void SetIndicatorState(List<GameObject> indicators, int activeCount)
    {
        if (indicators == null)
            return;

        for (int i = 0; i < indicators.Count; i++)
        {
            if (indicators[i] != null)
                indicators[i].SetActive(i < activeCount);
        }
    }

    private void GetNeedleBounds(out float minY, out float maxY)
    {
        if (TryGetLocalBoundsFor(needle, out Bounds bounds))
        {
            float halfHeight = needle.rect.height * 0.5f;
            minY = bounds.min.y + halfHeight;
            maxY = bounds.max.y - halfHeight;
            return;
        }

        minY = Mathf.Min(movementMinY, movementMaxY);
        maxY = Mathf.Max(movementMinY, movementMaxY);
    }

    private void GetSuccessZoneBounds(out float minY, out float maxY)
    {
        if (TryGetLocalBoundsFor(successZone, out Bounds bounds))
        {
            float halfHeight = successZone.rect.height * 0.5f;
            minY = bounds.min.y + halfHeight;
            maxY = bounds.max.y - halfHeight;
            return;
        }

        float minRange = Mathf.Min(successZoneYRange.x, successZoneYRange.y);
        float maxRange = Mathf.Max(successZoneYRange.x, successZoneYRange.y);
        float halfZone = successZone.rect.height * 0.5f;

        minY = Mathf.Max(minRange, Mathf.Min(movementMinY, movementMaxY) + halfZone);
        maxY = Mathf.Min(maxRange, Mathf.Max(movementMinY, movementMaxY) - halfZone);
    }

    private bool TryGetLocalBoundsFor(RectTransform target, out Bounds bounds)
    {
        bounds = default;

        if (!useQteBarAreaBounds || qteBarArea == null || target == null)
            return false;

        RectTransform targetParent = target.parent as RectTransform;
        if (targetParent == null)
            return false;

        bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(targetParent, qteBarArea);
        return bounds.size != Vector3.zero;
    }

    private Camera GetUICamera()
    {
        if (parentCanvas == null)
            return null;

        return parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera;
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
