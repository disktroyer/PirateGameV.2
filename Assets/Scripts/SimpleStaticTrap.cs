using UnityEngine;

public class SimpleStaticTrap : Interactable
{
    [Header("Configuración")]
    public ItemData requiredItem;   // Ejemplo: "Veneno", "Anguila", "Fregona"
    public int damage = 1;
    public AudioClip prepareSound;
    public AudioClip triggerSound;
    private bool isPrepared = false;

    public override void Interact(GameObject player)
    {

        InventoryManager inventory = player.GetComponent<InventoryManager>();
        print($"entro tengo item? {inventory.ContieneItem(requiredItem)}");
        if (inventory.ContieneItem(requiredItem))
        {
            isPrepared = true;
            AudioSource.PlayClipAtPoint(prepareSound, transform.position);
            Debug.Log($"{name} preparado con {requiredItem}");
        }
        else
        {
            Debug.Log($"Falta {requiredItem}");
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
            AudioSource.PlayClipAtPoint(triggerSound, transform.position);
            Debug.Log($"{name} activado: -{damage} HP al Boss");
        }
    }
}
