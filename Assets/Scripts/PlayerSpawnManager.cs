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

    [Header("Control Scripts")]
    public MonoBehaviour movementScript;
    public MonoBehaviour interactionScript;
    public MonoBehaviour inventoryScript;
    public MonoBehaviour craftingScript;
    public MonoBehaviour qteScript;

    void Start()
    {
        DisableAll();

        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
        }

        if (animator != null)
        {
            animator.SetBool(spawnBool, false);
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
        // Activar controles cuando la animación de spawn haya terminado
        EnableAll();

        // Marcar flag en el animator por compatibilidad con otros sistemas
        if (animator != null)
            animator.SetBool(spawnBool, true);

        Debug.Log("✓ Spawn completado. Controles del jugador activados.");
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

        bool enteredSpawn = false;
        while (true)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName(spawnStateName))
            {
                enteredSpawn = true;
            }

            if (enteredSpawn && !state.IsName(spawnStateName))
            {
                break;
            }

            // Si el clip ya alcanzó el final una vez, damos por terminado el spawn
            if (state.IsName(spawnStateName) && state.normalizedTime >= 1f)
            {
                break;
            }

            yield return null;
        }

        // Pequeña espera adicional para asegurarse de que la transición terminó
        yield return null;

        OnSpawnComplete();
    }
}
