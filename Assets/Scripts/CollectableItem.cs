using UnityEngine;

// Hereda de tu clase abstracta Interactable
public class CollectableItem : Interactable
{
    [Header("Datos del Objeto")]
    public ItemData itemData;

    // El jugador interactúa con el objeto (presiona E)
    public override void Interact(GameObject player)
    {
        if (itemData == null)
        {
            Debug.LogWarning("Este objeto no tiene un ItemData asignado.");
            return;
        }

        InventoryManager inventory = player.GetComponent<InventoryManager>();

        if (inventory != null)
        {
            bool added = inventory.AddItem(itemData);

            if (added)
            {
                Debug.Log($"Recogiste: {itemData.itemName}");
                Destroy(gameObject); // Elimina el objeto del mundo
            }
            else
            {
                Debug.Log("Inventario lleno.");
            }
        }
        else
        {
            Debug.LogWarning("El jugador no tiene un InventoryManager asignado.");
        }
    }
}
