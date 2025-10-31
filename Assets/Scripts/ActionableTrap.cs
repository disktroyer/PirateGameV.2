using UnityEngine;

public class ActionableTrap : Interactable
{
    public ItemData requiredItem;   // "Pólvora" o "Cuchillo"
    //public Transform effectZone;  // dónde verificar si el Boss está (para cañón)
    //public float effectRadius = 5f;

    public GameObject spawnTrap;
    public Transform spawnPoint;

    public override void Interact(GameObject actor)
    {
        var inv = actor.GetComponent<InventoryManager>();
        if (inv == null || !inv.ContieneItem(requiredItem))
        {
            Debug.Log($"Falta {requiredItem}");
            return;
        }

        Instantiate(spawnTrap, spawnPoint.position, Quaternion.identity);

        //esto es logica trampa
        // Ejemplo cañón: daño si Boss está en zona
        //var boss = FindObjectOfType<BossHealth>();
        //if (boss != null)
        //{
        //    bool bossEnZona = Vector3.Distance(boss.transform.position, effectZone.position) <= effectRadius;
        //    if (bossEnZona)
        //    {
        //        boss.RecibirDaño(damage);
        //        Debug.Log($"Cañón impacta: -{damage} HP");
        //    }
        //    else
        //    {
        //        Debug.Log("Cañón dispara, pero no hay Boss en zona");
        //    }
        //}

        // Ejemplo farolillo: marcar fuego 2h (puedes simular con un timer del juego)
        // StartCoroutine(FuegoTemporal(2f*60f)); // según tu escala de tiempo
    }
}
 