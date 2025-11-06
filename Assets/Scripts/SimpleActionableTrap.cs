using UnityEngine;

public class SimpleActionableTrap : MonoBehaviour
{

    [Header("Configuración")]
    public string requiredItem;   // Ejemplo: "P�lvora", "Cuchillo"
    public int damage = 1;
    public BossHealth boss;
    public AudioClip useSound;

    public void Activate(InventoryManager inv)
    {
        if (inv.ContieneItem(requiredItem))
        {
            if (useSound != null)
                AudioSource.PlayClipAtPoint(useSound, transform.position);

            if (boss != null)
            {
                boss.RecibirDaño(damage);
                Debug.Log($"Trampa activada: -{damage} HP al Boss");
            }
        }
        else
        {
            Debug.Log($"Falta {requiredItem} para activar esta trampa");
        }
    }
}
