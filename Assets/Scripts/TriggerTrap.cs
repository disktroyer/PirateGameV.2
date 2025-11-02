using UnityEngine;

public class TriggerTrap : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        BossHealth health = other.GetComponent<BossHealth>();
        if (health != null)
        {
            health.RecibirDaño(damage);
            Debug.Log($"Trampa activada: -{damage} HP a {other.name}");
        }
    }
}
