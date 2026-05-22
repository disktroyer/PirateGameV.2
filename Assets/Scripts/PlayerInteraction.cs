using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interacción")]
    [Range(0.1f, 5f)]
    [Tooltip("Distancia máxima para detectar objetos interactuables.")]
    public float interactRange = 2.5f;
    [Tooltip("Capa usada para filtrar los objetos interactuables.")]
    public LayerMask interactableMask = Physics2D.AllLayers;
    public KeyCode interactKey = KeyCode.E;
    public GameObject interactionIndicator; // Sprite del botón de acción
    private Animator animator;



    void Start()
    {
        if (interactRange < 0.5f)
            interactRange = 2.5f;

        animator = GetComponent<Animator>();
        if (interactionIndicator != null)
            interactionIndicator.SetActive(false);
    }

    void Update()
    {
        UpdateInteractionIndicator();

        if (Input.GetKeyDown(interactKey))
            TryInteract();
    }

    void UpdateInteractionIndicator()
    {
        if (interactionIndicator == null) return;

        // Verificar si hay objetos interactuables en rango
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange, interactableMask);
        Interactable closestInteractable = null;
        float closestDistance = float.MaxValue;

        foreach (var hit in hits)
        {
            Interactable interactable = hit.GetComponent<Interactable>();
            if (interactable != null)
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestInteractable = interactable;
                }
            }
        }

        if (closestInteractable != null)
        {
            // Posicionar el indicador cerca del objeto más cercano
            Vector3 indicatorPosition = closestInteractable.transform.position + Vector3.up * 1.5f; // Ajusta la altura
            interactionIndicator.transform.position = indicatorPosition;
            interactionIndicator.SetActive(true);
        }
        else
        {
            interactionIndicator.SetActive(false);
        }
    }

    void TryInteract()
    {

        if (animator != null) animator.SetTrigger("agarrar");


        // Si está escondido, intenta salir del armario actual
        PlayerHideController hide = GetComponent<PlayerHideController>();
        if (hide != null && hide.IsHidden && hide.CurrentHideSpot != null)
        {
            hide.CurrentHideSpot.Interact(gameObject);
            UpdateInteractionIndicator(); // Actualizar indicador después de interactuar
            return;
        }

        // ✋ Si está dentro del armario pero NO hay CurrentHideSpot, no permitir interactuar
        if (hide != null && hide.IsHidden)
            return;

        // Si no está escondido, busca objetos interactuables
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange, interactableMask);
        foreach (var hit in hits)
        {
            if (hit == null)
                continue;

            Interactable interactable = hit.GetComponent<Interactable>();
            if (interactable != null)
            {
                interactable.Interact(gameObject);
                UpdateInteractionIndicator(); // Actualizar indicador después de interactuar
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
