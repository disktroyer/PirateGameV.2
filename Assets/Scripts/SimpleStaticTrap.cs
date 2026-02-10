using UnityEngine;

public class SimpleStaticTrap : Interactable
{
    [Header("Configuración")]
    public ItemData requiredItem;
    public int damage = 1;
    public AudioClip prepareSound;
    public AudioClip triggerSound;
    private bool isPrepared = false;

    public override void Interact(GameObject player)
    {

        Debug.Log($"PREPARACION TRAMPAA");
        InventoryManager inventory = player.GetComponent<InventoryManager>();

        if (inventory == null)
        {
            Debug.LogError("ERROR: El jugador no tiene InventoryManager.");
            return;
        }

        if (inventory.ContieneItem(requiredItem))
        {
            isPrepared = true;

            if (prepareSound != null)
                AudioSource.PlayClipAtPoint(prepareSound, transform.position);

            Debug.Log($"{name} preparado con {requiredItem.itemName}");
        }

        else
        {
            Debug.Log($"Falta {requiredItem.itemName}");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        
        if (!isPrepared) return;

        var boss = other.GetComponent<BossHealth>();
        if (boss != null)
        {
            boss.RecibirDaño(damage);
            isPrepared = false;

            if (triggerSound != null)
                AudioSource.PlayClipAtPoint(triggerSound, transform.position);

            Debug.Log($"{name} activado: -{damage} HP");
        }
    }
}
