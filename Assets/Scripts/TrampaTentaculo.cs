using UnityEngine;

public class TrampaTentaculo : MonoBehaviour
{
    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            activated = true;

            PlayerController player = other.GetComponent<PlayerController>();
            PlayerQTEController qte = other.GetComponent<PlayerQTEController>();

            if (player != null)
            {
                player.SetMovement(false);
            }

            if (qte != null)
            {
                // Arranca el QTE y pasa esta trampa como referencia
                qte.StartQTE(this);
            }
            else
            {
                Debug.LogError("No existe PlayerQTEController en el Player. Añádelo para que funcione el QTE.");
            }
        }
    }

    // Lo llama el PlayerQTEController cuando hay SUCCESS
    public void ReleasePlayer(GameObject playerObj)
    {
        PlayerController player = playerObj.GetComponent<PlayerController>();

        if (player != null)
            player.SetMovement(true);

        Destroy(gameObject);
    }
}