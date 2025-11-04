using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interacción")]
    public float interactRange = 2f;
    public KeyCode interactKey = KeyCode.E;

    private void Update()
    {
        // Detecta objetos interactuables al presionar la tecla E
        if (Input.GetKeyDown(interactKey))
        {
            DetectarObjetoInteractuable();
        }
    }

    void DetectarObjetoInteractuable()
    {
        // Lanza un círculo pequeño alrededor del jugador
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange);

        foreach (Collider2D hit in hits)
        {
            Interactable interactable = hit.GetComponent<Interactable>();

            if (interactable != null)
            {
                interactable.Interact(gameObject);
                return;
            }
        }
    }

    // Dibuja el rango de interacción en la escena (solo visible en el editor)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }


}
