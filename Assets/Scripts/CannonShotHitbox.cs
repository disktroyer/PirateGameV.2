using UnityEngine;

public class CannonShotHitbox : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 1;
    public string targetTag = "Enemy";
    public float lifeTime = 0.4f;

    private bool hasDamaged;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasDamaged)
            return;

        if (!other.CompareTag(targetTag))
            return;

        var bossController = other.GetComponent<BossController>();
        if (bossController != null)
        {
            bossController.RecibirDaño(damage);
            hasDamaged = true;
            return;
        }

        var bossHealth = other.GetComponent<BossHealth>();
        if (bossHealth != null)
        {
            bossHealth.RecibirDaño(damage);
            hasDamaged = true;
        }
    }
}