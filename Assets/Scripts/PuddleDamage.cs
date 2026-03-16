using UnityEngine;

public class PuddleDamage : MonoBehaviour
{
    public int damage = 1; // Daño que inflige al jefe

    private void OnTriggerEnter2D(Collider2D other)
    {
        var boss = other.GetComponent<BossHealth>();
        if (boss != null)
        {
            boss.RecibirDaño(damage);
            Debug.Log($"Charco activado: Jefe recibe -{damage} HP");
            // Opcional: Destruir el charco después de dañar
            // Destroy(gameObject);
        }
    }
}