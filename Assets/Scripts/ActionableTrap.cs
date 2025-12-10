//using UnityEngine;

//public class ActionableTrap : Interactable
//{
//    public ItemData requiredItem;   // "Polvora" o "Cuchillo"
//    //public Transform effectZone;  // donde verificar si el Boss est0 (para cañon)
//    //public float effectRadius = 5f;

//    public GameObject spawnTrap;
//    public Transform spawnPoint;

//    public override void Interact(GameObject actor)
//    {
//        var inv = actor.GetComponent<InventoryManager>();
//        if (inv == null || !inv.ContieneItem(requiredItem))
//        {
//            Debug.Log($"Falta {requiredItem}");
//            return;
//        }

//        Instantiate(spawnTrap, spawnPoint.position, Quaternion.identity);

//        //esto es logica trampa
//        // Ejemplo cañon: daño si Boss esto en zona
//        //var boss = FindObjectOfType<BossHealth>();
//        //if (boss != null)
//        //{
//        //    bool bossEnZona = Vector3.Distance(boss.transform.position, effectZone.position) <= effectRadius;
//        //    if (bossEnZona)
//        //    {
//        //        boss.RecibirDaño(damage);
//        //        Debug.Log($"Cañon impacta: -{damage} HP");
//        //    }
//        //    else
//        //    {
//        //        Debug.Log("Cañon dispara, pero no hay Boss en zona");
//        //    }
//        //}

//        // Ejemplo farolillo: marcar fuego 2h (puedes simular con un timer del juego)
//        // StartCoroutine(FuegoTemporal(2f*60f)); // segun tu escala de tiempo
//    }
//}
using UnityEngine;

public class ActionableTrap : Interactable
{
    public string requiredItem;   // "Polvora", "Cuchillo"
    public int damage = 1;

    public override void Interact(GameObject actor)
    {
        var inv = actor.GetComponent<InventoryManager>();
        if (!inv.ContieneItem(requiredItem))
        {
            Debug.Log($"Falta {requiredItem} para activar trampa");
            return;
        }

        // Accion inmediata
        var boss = FindObjectOfType<BossHealth>();
        if (boss != null)
        {
            boss.RecibirDaño(damage);
            Debug.Log($"Trampa accionada por jugador (-{damage} HP)");
        }
    }
}
