using UnityEngine;
using UnityEngine.UI;

public class PlaceableTrap : Interactable
{
    public string requiredItem;
    public GameObject trapPrefab;

    [Header("Placement")]
    public InventoryManager inventorySource;
    public Transform placementOrigin;
    public Vector2 placementOffset = new Vector2(0.8f, 0f);
    public bool useFacingDirection = true;

    [Header("UI")]
    public Button placeTrapButton;
    public bool refreshButtonState = true;
    public bool hideButtonWhenUnavailable = false;

    void Start()
    {
        if (inventorySource == null)
            inventorySource = GetComponent<InventoryManager>();

        RefreshButtonState();
    }

    void Update()
    {
        if (refreshButtonState)
            RefreshButtonState();
    }

    public override void Interact(GameObject actor)
    {
        TryPlaceTrap(actor != null ? actor.transform : null, actor != null ? actor.GetComponent<InventoryManager>() : null);
    }

    public void PlaceTrapFromButton()
    {
        Transform actorTransform = placementOrigin != null ? placementOrigin : (inventorySource != null ? inventorySource.transform : transform);
        TryPlaceTrap(actorTransform, inventorySource);
    }

    public void RefreshButtonState()
    {
        if (placeTrapButton == null)
            return;

        bool canPlace = inventorySource != null && inventorySource.ContieneItem(requiredItem);

        placeTrapButton.interactable = canPlace;

        if (hideButtonWhenUnavailable)
            placeTrapButton.gameObject.SetActive(canPlace);
    }

    private void TryPlaceTrap(Transform actorTransform, InventoryManager inv)
    {
        if (inv == null)
        {
            Debug.Log("PlaceableTrap: no hay InventoryManager asignado");
            RefreshButtonState();
            return;
        }

        if (trapPrefab == null)
        {
            Debug.Log("PlaceableTrap: falta asignar trapPrefab");
            RefreshButtonState();
            return;
        }

        if (!inv.ContieneItem(requiredItem))
        {
            Debug.Log("No tienes el ítem para colocar esta trampa");
            RefreshButtonState();
            return;
        }

        Transform origin = actorTransform != null ? actorTransform : transform;
        Vector3 spawnPosition = origin.position;

        Vector2 appliedOffset = placementOffset;
        if (useFacingDirection && origin.localScale.x < 0f)
            appliedOffset.x *= -1f;

        spawnPosition += new Vector3(appliedOffset.x, appliedOffset.y, 0f);

        Instantiate(trapPrefab, spawnPosition, Quaternion.identity);
        inv.EliminarItem(requiredItem);
        Debug.Log("Trampa colocada");
        RefreshButtonState();
    }
}
