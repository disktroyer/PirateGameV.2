using UnityEngine;

public class SimpleActionableTrap : MonoBehaviour
{

    [Header("Configuración")]
    public string requiredItem;   // Ejemplo: "P�lvora", "Cuchillo"
    public int damage = 1;
    public BossHealth boss;
    public AudioClip useSound;
    public bool directDamageOnActivate = false;

    [Header("Cañón")]
    public bool spawnFrontHitboxOnUse = true;
    public Vector2 hitboxSize = new Vector2(2.2f, 1.2f);
    public float hitboxDistance = 1.8f;
    public float hitboxLifeTime = 0.4f;
    public string enemyTag = "Enemy";
    public bool showHitboxGizmo = true;
    public Color gizmoColor = new Color(1f, 0.4f, 0f, 0.35f);

    public void Activate(InventoryManager inv)
    {
        if (inv == null)
            return;

        if (inv.ContieneItem(requiredItem))
        {
            if (useSound != null)
                AudioSource.PlayClipAtPoint(useSound, transform.position);

            if (spawnFrontHitboxOnUse)
                SpawnFrontHitbox();

            if (directDamageOnActivate && boss != null)
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
