using UnityEngine;

public class PuddleDamage : MonoBehaviour
{
    public int damage = 1; // Daño que inflige al jefe
    public float slipDuration = 2f; // Duración de la animación de resbalarse

    private void OnTriggerEnter2D(Collider2D other)
    {
        var bossHealth = other.GetComponent<BossHealth>();
        var bossController = other.GetComponent<BossController>();
        
        if (bossHealth != null)
        {
            bossHealth.RecibirDaño(damage);
            Debug.Log($"Charco activado: Jefe recibe -{damage} HP");
            
            // Activar animación de resbalarse si el BossController existe
            if (bossController != null)
            {
                bossController.Trap_Slip(slipDuration);
                Debug.Log($"Charco: Jefe se resbala por {slipDuration}s");
            }
            
            // Opcional: Destruir el charco después de dañar
            // Destroy(gameObject);
        }
    }
}