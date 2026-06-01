using UnityEngine;

public class BottleRonTrap : MonoBehaviour
{
    [Header("Daño")]
    public float damage = 2f;
    public float stunDuration = 2.5f;
    public string poisonTrigger = "Poisoned"; // Animación de envenenado

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
                Debug.Log($"BottleRon: Boss recibe -{damage} HP");
            }

            // Reproducir animación de envenenado
            if (bossController != null)
            {
                bossController.Trap_Stun(stunDuration, poisonTrigger);
                Debug.Log($"BottleRon: Boss envenenado por {stunDuration}s");
            }

            // Opcional: destruir la botella después de usarla
            Destroy(gameObject, 0.1f);
        }
    }
}
