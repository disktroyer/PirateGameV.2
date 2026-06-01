using UnityEngine;

public class BarrilTrap : MonoBehaviour
{
    [Header("Daño")]
    public float damage = 1f;
    public float slipDuration = 2.5f;

    [Header("Detección")]
    public string bossTag = "Boss";

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        // Detectar jefe
        BossController bossController = other.GetComponent<BossController>();
        BossHealth bossHealth = other.GetComponent<BossHealth>();

        if (bossController != null || bossHealth != null)
        {
            hasTriggered = true;

            // Causar daño
            if (bossHealth != null)
            {
                bossHealth.RecibirDaño(damage);
                Debug.Log($"Barril: Boss recibe -{damage} HP");
            }

            // Reproducir animación de resbalarse
            if (bossController != null)
            {
                bossController.Trap_Slip(slipDuration);
                Debug.Log($"Barril: Boss se resbala por {slipDuration}s");
            }

            // Opcional: destruir el barril después de usarlo
            Destroy(gameObject, 0.1f);
        }
    }
}
