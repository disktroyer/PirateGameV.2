using UnityEngine;
using UnityEngine.UI;

public class PlayerQTEController : MonoBehaviour
{
    [Header("QTE Settings")]
    public KeyCode[] qteKeys = { KeyCode.E, KeyCode.F, KeyCode.Space };
    public int pressesRequired = 5;       // Cuántas pulsaciones para escapar
    public float timeLimit = 5f;          // Tiempo máximo para escapar

    [Header("References")]
    public Animator playerAnimator;       // Animator del jugador (mismo GameObject)
    public GameObject qteUI;              // Panel de UI del QTE (opcional)
    public Slider progressBar;            // Barra de progreso (opcional)

    private TentacleTrap currentTrap;
    private bool isInQTE = false;
    private int pressCount = 0;
    private float timer = 0f;

    private void Awake()
    {
        // Obtener el Animator del mismo GameObject si no está asignado
        if (playerAnimator == null)
            playerAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isInQTE) return;

        timer -= Time.deltaTime;

        // Actualizar barra de progreso si existe
        if (progressBar != null)
            progressBar.value = (float)pressCount / pressesRequired;

        // Tiempo agotado: reiniciar progreso
        if (timer <= 0f)
        {
            pressCount = 0;
            timer = timeLimit;
            return;
        }

        // Detectar cualquier tecla del QTE
        foreach (KeyCode key in qteKeys)
        {
            if (Input.GetKeyDown(key))
            {
                pressCount++;

                if (pressCount >= pressesRequired)
                {
                    Escape();
                    return;
                }
            }
        }
    }

    public void StartQTE(TentacleTrap trap)
    {
        if (isInQTE) return;

        currentTrap = trap;
        isInQTE = true;
        pressCount = 0;
        timer = timeLimit;

        // Activar animación de atado en el jugador
        if (playerAnimator != null)
            playerAnimator.SetTrigger("AtadoTentaculo");

        // Mostrar UI del QTE si existe
        if (qteUI != null)
            qteUI.SetActive(true);
    }

    private void Escape()
    {
        isInQTE = false;

        // Volver a animación normal del jugador
        if (playerAnimator != null)
            playerAnimator.SetTrigger("Idle");

        // Ocultar UI
        if (qteUI != null)
            qteUI.SetActive(false);

        // Avisar a la trampa para que libere al jugador
        if (currentTrap != null)
            currentTrap.ReleasePlayer(gameObject);

        currentTrap = null;
    }

    // Called by external QTE UI (like QTEWheelUI) when the player succeeds
    public void OnSuccess()
    {
        Escape();
    }

    // Called by external QTE UI when the player fails the QTE input
    public void OnFail()
    {
        // End the immediate QTE UI interaction but keep the player trapped.
        isInQTE = false;

        // Hide QTE UI if present
        if (qteUI != null)
            qteUI.SetActive(false);

        // Reset counters so a future QTE starts clean
        pressCount = 0;
        timer = timeLimit;

        // Optionally play a failure animation or keep the "AtadoTentaculo" state
        if (playerAnimator != null)
            playerAnimator.SetTrigger("AtadoTentaculo");
    }
}