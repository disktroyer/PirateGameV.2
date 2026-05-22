using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class PlaceableTrap : Interactable
{
    [Header("Datos de colocación")]
    public TrapData trapData;
    public ItemData requiredItem;
    public GameObject trapPrefab;
    public GameObject previewPrefab;

    [Header("Restricciones de colocación")]
    [Tooltip("Capa donde el suelo es válido para colocar la trampa.")]
    public LayerMask placementMask = ~0;
    [Tooltip("Capa que bloquea la colocación si hay colisión.")]
    public LayerMask obstacleMask = 0;
    [Tooltip("Distancia máxima desde el jugador para colocar la trampa.")]
    public float maxPlaceDistance = 4f;
    public bool snapToGrid = true;
    public float gridSize = 1f;

    [Header("Eventos")]
    public UnityEvent onPlacementStarted;
    public UnityEvent onPlaced;

    private bool isPlacing = false;
    private GameObject previewInstance;
    private InventoryManager activeInventory;

    public override void Interact(GameObject actor)
    {
        if (actor == null)
            return;

        InventoryManager inventory = actor.GetComponent<InventoryManager>();
        if (inventory == null)
        {
            Debug.Log("InventoryManager no encontrado en el jugador.");
            return;
        }

        if (requiredItem != null && !inventory.HasItem(requiredItem))
        {
            Debug.Log("No tienes el item requerido para colocar esta trampa.");
            return;
        }

        if (trapPrefab == null)
        {
            Debug.LogWarning("trapPrefab no asignado en PlaceableTrap.");
            return;
        }

        StartPlacement(inventory);
    }

    private void Update()
    {
        if (!isPlacing || previewInstance == null)
            return;

        Vector3 mouseWorld = GetMouseWorldPosition();
        Vector3 placementPosition = snapToGrid ? SnapToGrid(mouseWorld) : mouseWorld;

        if (Vector2.Distance(activeInventory.transform.position, placementPosition) > maxPlaceDistance)
        {
            placementPosition = activeInventory.transform.position + (placementPosition - activeInventory.transform.position).normalized * maxPlaceDistance;
        }

        previewInstance.transform.position = placementPosition;
        UpdatePreviewVisual(placementPosition);

        if (Input.GetMouseButtonDown(0) && CanPlaceAt(placementPosition))
        {
            PlaceTrap(placementPosition);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
        }
    }

    private void StartPlacement(InventoryManager inventory)
    {
        activeInventory = inventory;
        isPlacing = true;

        GameObject template = previewPrefab != null ? previewPrefab : trapPrefab;
        previewInstance = Instantiate(template, inventory.transform.position, Quaternion.identity);
        SetPreviewAlpha(previewInstance, 0.5f);

        onPlacementStarted?.Invoke();
        Debug.Log("Modo colocación activado para " + trapData?.trapName);
    }

    private void PlaceTrap(Vector3 position)
    {
        GameObject placedTrap = Instantiate(trapPrefab, position, Quaternion.identity);
        if (trapData != null)
        {
            TrapInteractable trap = placedTrap.GetComponent<TrapInteractable>();
            if (trap != null)
            {
                trap.trapData = trapData;
                if (trapData.trapType == TrapType.PLACEABLE)
                    trap.ForcePrepare();
            }
        }

        if (requiredItem != null)
        {
            activeInventory.ConsumeItem(requiredItem);
            Debug.Log("Item consumido al colocar la trampa: " + requiredItem.itemName);
        }

        onPlaced?.Invoke();
        Debug.Log("Trampa colocada en: " + position);
        EndPlacement();
    }

    private void CancelPlacement()
    {
        Debug.Log("Colocación cancelada.");
        EndPlacement();
    }

    private void EndPlacement()
    {
        isPlacing = false;
        if (previewInstance != null)
            Destroy(previewInstance);

        previewInstance = null;
        activeInventory = null;
    }

    private bool CanPlaceAt(Vector3 position)
    {
        bool validSurface = placementMask == 0 || Physics2D.OverlapPoint(position, placementMask) != null;
        bool noObstacle = obstacleMask == 0 || Physics2D.OverlapCircle(position, 0.25f, obstacleMask) == null;
        return validSurface && noObstacle;
    }

    private void UpdatePreviewVisual(Vector3 position)
    {
        bool valid = CanPlaceAt(position);
        SpriteRenderer sprite = previewInstance.GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            Color color = valid ? new Color(0.3f, 1f, 0.3f, 0.5f) : new Color(1f, 0.3f, 0.3f, 0.5f);
            sprite.color = color;
        }
    }

    private void SetPreviewAlpha(GameObject preview, float alpha)
    {
        if (preview == null)
            return;

        SpriteRenderer sprite = preview.GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            Color color = sprite.color;
            color.a = alpha;
            sprite.color = color;
        }

        ParticleSystem particles = preview.GetComponent<ParticleSystem>();
        if (particles != null)
            particles.Stop();
    }

    private Vector3 GetMouseWorldPosition()
    {
        Camera cam = Camera.main;
        if (cam == null)
            cam = Camera.current;

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 0f;
        Vector3 worldPosition = cam.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0f;
        return worldPosition;
    }

    private Vector3 SnapToGrid(Vector3 position)
    {
        return new Vector3(
            Mathf.Round(position.x / gridSize) * gridSize,
            Mathf.Round(position.y / gridSize) * gridSize,
            0f
        );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, maxPlaceDistance);
    }
}
