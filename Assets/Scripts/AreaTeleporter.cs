using UnityEngine;

public class AreaTeleporter : MonoBehaviour
{
    [Header("Destino del teletransporte")]
    public Transform targetPosition;

    [Header("Opciones")]
    public bool allowBoss = false;
    public float cooldown = 0.5f; // medio segundo de protección

    private static float lastTeleportTime = -1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Evitar bucle: si pasó menos de X segundos, no teletransportamos
        if (Time.time - lastTeleportTime < cooldown)
            return;

        if (other.CompareTag("Player") || (allowBoss && other.CompareTag("Boss")))
        {
            other.transform.position = targetPosition.position;
            lastTeleportTime = Time.time;  // Actualiza tiempo del último TP
        }
    }
}
