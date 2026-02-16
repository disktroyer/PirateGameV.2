using UnityEngine;

public class TrampaTentaculo : MonoBehaviour
{
    public QTEController qteController;
    public float trapDuration = 6f;

    private PlayerController trappedPlayer;
    private bool isActive = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActive) return;

        if (other.CompareTag("Player"))
        {
            isActive = true;

            trappedPlayer = other.GetComponent<PlayerController>();

            if (trappedPlayer != null)
            {
                trappedPlayer.SetMovement(false);

                // Configurar eventos dinámicamente
                qteController.onSuccess.RemoveAllListeners();
                qteController.onFail.RemoveAllListeners();

                qteController.onSuccess.AddListener(ReleasePlayer);
                qteController.onFail.AddListener(FailQTE);

                qteController.StartQTE();
            }
        }
    }

    void ReleasePlayer()
    {
        if (trappedPlayer != null)
        {
            trappedPlayer.SetMovement(true);
        }

        Destroy(gameObject);
    }

    void FailQTE()
    {
        // Aquí decides qué pasa si falla:
        // opción 1: volver a intentar automáticamente
        qteController.StartQTE();

        // opción 2: daño
        // trappedPlayer.TakeDamage(10);

        // opción 3: tiempo extra atrapado
    }
}
