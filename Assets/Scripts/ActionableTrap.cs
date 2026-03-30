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
    public bool directDamageOnActivate = true;

    [Header("Cañón")]
    public bool spawnFrontHitboxOnUse = false;
    public Vector2 hitboxSize = new Vector2(2.2f, 1.2f);
    public float hitboxDistance = 1.8f;
    public float hitboxLifeTime = 0.4f;
    public string enemyTag = "Enemy";
    public bool showHitboxGizmo = true;
    public Color gizmoColor = new Color(1f, 0.4f, 0f, 0.35f);

    public override void Interact(GameObject actor)
    {
        var inv = actor.GetComponent<InventoryManager>();
        if (inv == null || !inv.ContieneItem(requiredItem))
        {
            Debug.Log($"Falta {requiredItem} para activar trampa");
            return;
        }

        if (spawnFrontHitboxOnUse)
            SpawnFrontHitbox();

        // Accion inmediata
        var boss = FindObjectOfType<BossHealth>();
        if (directDamageOnActivate && boss != null)
        {
            boss.RecibirDaño(damage);
            Debug.Log($"Trampa accionada por jugador (-{damage} HP)");
        }
    }

    private void SpawnFrontHitbox()
    {
        var hitboxObject = new GameObject("CannonShotHitbox");
        hitboxObject.transform.position = (Vector2)transform.position + ((Vector2)transform.right * hitboxDistance);
        hitboxObject.transform.rotation = transform.rotation;

        var boxCollider = hitboxObject.AddComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
        boxCollider.size = hitboxSize;

        var rb = hitboxObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = true;

        var hitbox = hitboxObject.AddComponent<CannonShotHitbox>();
        hitbox.damage = damage;
        hitbox.targetTag = enemyTag;
        hitbox.lifeTime = hitboxLifeTime;
    }

    private void OnDrawGizmosSelected()
    {
        if (!showHitboxGizmo || !spawnFrontHitboxOnUse)
            return;

        Vector3 center = transform.position + (transform.right * hitboxDistance);
        Vector3 size = new Vector3(hitboxSize.x, hitboxSize.y, 0.05f);

        Gizmos.color = gizmoColor;
        Matrix4x4 previousMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, size);
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);
        Gizmos.DrawWireCube(Vector3.zero, size);
        Gizmos.matrix = previousMatrix;
    }
}
