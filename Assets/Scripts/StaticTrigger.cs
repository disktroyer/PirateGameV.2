using System.Xml.Linq;
using UnityEngine;

public class StaticTrigger : MonoBehaviour
{
    public string requiredPrepItem;   // "Veneno", "Anguila", "Fregona y cubo"
    public int damage = 1;
    public bool isPrimed = false;

    public void Preparar(InventoryManager inv)
    {
        if (inv.ContieneItem(requiredPrepItem))
        {
            isPrimed = true;
            // opcional: consumir ítem / generar ruido
            Debug.Log($"{name} preparado con {requiredPrepItem}");
        }
        else Debug.Log($"Falta {requiredPrepItem}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isPrimed) return;

        var boss = other.GetComponent<BossHealth>();
        if (boss != null)
        {
            boss.RecibirDaño(damage);
            isPrimed = false; // o destruir si es de un solo uso
            Debug.Log($"{name} activado sobre el Boss (-{damage} HP)");
        }
    }
}
