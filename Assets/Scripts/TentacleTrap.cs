using UnityEngine;

public class TentacleTrap : MonoBehaviour
{
    public float slowMultiplier = 0.2f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerQTEController qte = other.GetComponent<PlayerQTEController>();
        PlayerMovement movement = other.GetComponent<PlayerMovement>();

        if (movement != null)
            movement.SetSpeedMultiplier(slowMultiplier);

        if (qte != null)
            qte.StartQTE(this);
    }

    public void ReleasePlayer(GameObject player)
    {
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
            movement.ResetSpeed();

        Destroy(gameObject);
    }
}
