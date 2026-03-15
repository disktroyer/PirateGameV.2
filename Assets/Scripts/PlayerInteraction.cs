using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interacci�n")]
    public float interactRange = 2f;
    public KeyCode interactKey = KeyCode.E;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        // Detecta objetos interactuables al presionar la tecla E
        if (Input.GetKeyDown(interactKey))
        {
            //xanimator.SetTrigger("Agarrar");

            DetectarObjetoInteractuable();
        }
    }

    void DetectarObjetoInteractuable()
    {
        // Lanza un c�rculo peque�o alrededor del jugador
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange);

        foreach (Collider2D hit in hits)
        {
            print(hit.gameObject.name);
            Interactable interactable = hit.GetComponent<Interactable>();

            if (interactable != null)
            {
                animator.SetTrigger("agarrar");
                interactable.Interact(gameObject);
                return;
            }
        }
    }

    // Dibuja el rango de interacci�n en la escena (solo visible en el editor)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }

  

}
