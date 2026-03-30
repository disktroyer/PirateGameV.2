using System.Collections;
using UnityEngine;

public class PlacedTrapDamageOverTime : MonoBehaviour
{
    [Header("Target")]
    public string enemyTag = "Enemy";

    [Header("Effect")]
    public float effectDuration = 2f;
    public float damagePerTick = 1f;
    public float tickInterval = 0.5f;

    [Header("Lifecycle")]
    public bool destroyAfterTrigger = true;
    public float destroyDelay = 0.1f;

    private bool triggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered)
            return;

        if (!other.CompareTag(enemyTag))
            return;

        triggered = true;

        BossController bossController = other.GetComponent<BossController>();
        if (bossController != null)
            bossController.Trap_Stun(effectDuration);

        StartCoroutine(ApplyDamageOverTime(other));
    }

    private IEnumerator ApplyDamageOverTime(Collider2D target)
    {
        float elapsed = 0f;

        while (elapsed < effectDuration)
        {
            ApplyDamage(target);
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }

        if (destroyAfterTrigger)
            Destroy(gameObject, destroyDelay);
    }

    private void ApplyDamage(Collider2D target)
    {
        BossController bossController = target.GetComponent<BossController>();
        if (bossController != null)
        {
            bossController.RecibirDaño(damagePerTick);
            return;
        }

        BossHealth bossHealth = target.GetComponent<BossHealth>();
        if (bossHealth != null)
            bossHealth.RecibirDaño(damagePerTick);
    }
}