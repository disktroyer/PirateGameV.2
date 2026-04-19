using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 2.5f;
    public KeyCode interactKey = KeyCode.E;
    private Animator animator;



    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (Input.GetKeyDown(interactKey))
            TryInteract();
    }

    void TryInteract()
    {

        if (animator != null) animator.SetTrigger("agarrar");


        // Si está escondido, intenta salir del armario actual
        PlayerHideController hide = GetComponent<PlayerHideController>();
        if (hide != null && hide.IsHidden && hide.CurrentHideSpot != null)
        {
            hide.CurrentHideSpot.Interact(gameObject);
            return;
        }

        // ✋ Si está dentro del armario pero NO hay CurrentHideSpot, no permitir interactuar
        if (hide != null && hide.IsHidden)
            return;

        // Si no está escondido, busca objetos interactuables
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange);
        foreach (var hit in hits)
        {
            Interactable interactable = hit.GetComponent<Interactable>();
            if (interactable != null)
            {
                interactable.Interact(gameObject);
                return;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
