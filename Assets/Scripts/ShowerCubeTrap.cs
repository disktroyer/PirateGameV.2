using UnityEngine;

public class ShowerCubeTrap : Interactable
{
    [Header("Trampa de Electrocución")]
    public ElectrocutionTrap electrocutionTrap;

    [Header("Consumo")]
    public string requiredItem = "anguila electrica";
    public bool consumeItem = true;

    [Header("Visual Change")]
    public SpriteRenderer spriteRenderer;
    public Sprite activatedSprite; // Sprite del cubo con anguila dentro

    public override void Interact(GameObject actor)
    {
        var inv = actor.GetComponent<InventoryManager>();
        if (inv == null)
        {
            Debug.Log("No se encontró InventoryManager");
            return;
        }

        // Debug: Imprimir todos los items en el inventario
        Debug.Log("Items en inventario:");
        for (int i = 0; i < inv.items.Length; i++)
        {
            if (inv.items[i] != null)
                Debug.Log($"Slot {i}: {inv.items[i].itemName}");
            else
                Debug.Log($"Slot {i}: vacío");
        }

        if (!inv.ContieneItem(requiredItem))
        {
            Debug.Log($"Necesitas {requiredItem} para activar esta trampa. Item requerido: '{requiredItem}'");
            return;
        }

        // Cambiar sprite al activado
        if (spriteRenderer != null && activatedSprite != null)
        {
            spriteRenderer.sprite = activatedSprite;
        }

        // Activar la trampa de electrocución
        if (electrocutionTrap != null)
        {
            electrocutionTrap.Activate();
        }
        else
        {
            Debug.LogError("ShowerCubeTrap: ElectrocutionTrap no asignada");
            return;
        }

        // Consumir la anguila
        if (consumeItem)
        {
            for (int i = 0; i < inv.items.Length; i++)
            {
                if (inv.items[i] != null && inv.items[i].itemName == requiredItem)
                {
                    inv.items[i] = null;
                    inv.ActualizarUI();
                    Debug.Log($"Anguila consumida");
                    return;
                }
            }
        }
    }
}
