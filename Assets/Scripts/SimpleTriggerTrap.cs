using UnityEngine;

public class SimpleTriggerTrap : MonoBehaviour
{
    [Header("Configuración")]
    public int damage = 1;
    public AudioClip triggerSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var boss = other.GetComponent<BossHealth>();
        if (boss != null)
        {
            boss.RecibirDaño(damage);
            AudioSource.PlayClipAtPoint(triggerSound, transform.position);
            Debug.Log($"Trampa autom�ítica: -{damage} HP al Boss");
            Destroy(gameObject);
        }
    }
}
