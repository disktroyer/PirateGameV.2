using UnityEngine;
using System.Collections;

public class PlayerSpawnController : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;
    public string spawnStateName = "Spawn";

    [Header("Visual")]
    public SpriteRenderer spriteRenderer;

    [Header("Control Scripts")]
    public MonoBehaviour movementScript;
    public MonoBehaviour interactionScript; 
    public MonoBehaviour inventoryScript;
    public MonoBehaviour craftingScript;
    public MonoBehaviour qteScript;

    private bool hasStarted = false;

    void Start()
    {
        // Obtener SpriteRenderer si no está asignado
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        // Bloquear todo desde el inicio
        DisableAll();

        // Ocultar el jugador al inicio
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        // Iniciar animación de spawn
        if (animator != null)
        {
            animator.Play(spawnStateName);
            StartCoroutine(WaitForSpawnEnd());
        }
        else
        {
            // Si no hay animator, mostrar jugador y activar controles de inmediato
            ShowPlayer();
            EnableAll();
        }
    }

    IEnumerator WaitForSpawnEnd()
    {
        // Espera hasta que la animación de spawn **ya no esté activa**
        hasStarted = true;

        // Mientras el animator esté reproduciendo "Spawn" → espera
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(spawnStateName))
        {
            yield return null;
        }

        // Terminado el estado de spawn → mostrar jugador y reactivar controles
        ShowPlayer();
        EnableAll();
        
        Debug.Log("✓ Spawn completado. Jugador visible y controles activados.");
    }

    void ShowPlayer()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
    }

    void DisableAll()
    {
        if (movementScript != null) movementScript.enabled = false;
        if (interactionScript != null) interactionScript.enabled = false;
        if (inventoryScript != null) inventoryScript.enabled = false;
        if (craftingScript != null) craftingScript.enabled = false;
        if (qteScript != null) qteScript.enabled = false;
    }

    void EnableAll()
    {
        if (movementScript != null) movementScript.enabled = true;
        if (interactionScript != null) interactionScript.enabled = true;
        if (inventoryScript != null) inventoryScript.enabled = true;
        if (craftingScript != null) craftingScript.enabled = true;
        if (qteScript != null) qteScript.enabled = true;
    }
}
