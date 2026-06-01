using UnityEngine;
using System.Collections;

public class PlayerSpawnManager : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;
    public string spawnBool = "HasSpawnFinished";
    public string spawnStateName = "Spawn"; // nombre del estado de spawn en el Animator

    [Header("Spawn Point")]
    public Transform spawnPoint; // Asignar SpawnSpriteSheet_0 aquí
    public GameObject spawnVisual; // Animación de spawn visible

    [Header("Visual")]
    public SpriteRenderer spriteRenderer;

    [Header("Control Scripts")]
    public MonoBehaviour movementScript;
    public MonoBehaviour interactionScript;
    public MonoBehaviour inventoryScript;
    public MonoBehaviour craftingScript;
    public MonoBehaviour qteScript;

    void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        DisableAll();

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        if (spawnVisual != null)
        {
            spawnVisual.SetActive(true);
            if (animator == null)
                animator = spawnVisual.GetComponent<Animator>();
        }

        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
        }

        if (animator != null)
        {
            animator.SetBool(spawnBool, false);
            // Si el spawnVisual contiene el Animator con el estado por defecto, arrancamos la comprobación
            StartCoroutine(WaitForSpawnEnd());
        }
    }

    void Update()
    {
        if (animator == null) return;

        // El flujo de finalización del spawn se hace en OnSpawnComplete() llamado por la corrutina
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

    public void OnSpawnComplete()
    {
        // Mostrar sprite del jugador
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        // Ocultar visual del spawn
        if (spawnVisual != null)
            spawnVisual.SetActive(false);

        // Activar controles
        EnableAll();

        // Marcar flag en el animator por compatibilidad con otros sistemas
        if (animator != null)
            animator.SetBool(spawnBool, true);

        Debug.Log("✓ Spawn completado. Jugador visible y controles activados.");
    }

    IEnumerator WaitForSpawnEnd()
    {
        if (animator == null)
            yield break;

        // Esperar a que el Animator entre en el estado de spawn (por si no está todavía)
        int attempts = 0;
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(spawnStateName) && attempts < 300)
        {
            attempts++;
            yield return null;
        }

        // Si no encontramos el estado, esperar un breve periodo y finalizar
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(spawnStateName))
        {
            // timeout: activar directamente
            OnSpawnComplete();
            yield break;
        }

        // Ahora esperar a que el estado deje de estar activo (la animación termine)
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(spawnStateName))
        {
            yield return null;
        }

        // Pequeña espera adicional para asegurarse de que la transición terminó
        yield return null;

        OnSpawnComplete();
    }
}
