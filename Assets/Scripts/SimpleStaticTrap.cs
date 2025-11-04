using UnityEngine;

public class SimpleStaticTrap : MonoBehaviour
{
    [Header("Configuración")]
    public string requiredItem;   // Ejemplo: "Veneno", "Anguila", "Fregona"
    public int damage = 1;
    public AudioClip prepareSound;
    public AudioClip triggerSound;
    private bool isPrepared = false;

    public void Prepare(InventoryManager inv)
    {
        if (inv.ContieneItem(requiredItem))
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
