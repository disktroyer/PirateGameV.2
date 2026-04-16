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
    public QTEController qteController;    // Controlador de barra vertical QTE

    private PlayerController playerController;
    private PlayerMovement playerMovement;
    private Rigidbody2D playerRb;

    private TentacleTrap currentTrap;
    private bool isInQTE = false;
    private int pressCount = 0;
    private float timer = 0f;

    private void Awake()
    {
        // Obtener el Animator del mismo GameObject si no está asignado
        if (playerAnimator == null)
            playerAnimator = GetComponent<Animator>();

        playerController = GetComponent<PlayerController>();
        playerMovement = GetComponent<PlayerMovement>();
        playerRb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!isInQTE) return;

        if (qteController != null)
            return; // El QTE visual se maneja en QTEController.

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

        SetPlayerLocked(true);

        // Activar animación de atado en el jugador
        if (playerAnimator != null)
            playerAnimator.SetTrigger("AtadoTentaculo");

        // Mostrar UI del QTE si existe
        if (qteUI != null)
            qteUI.SetActive(true);

        if (qteController != null)
        {
            qteController.onSuccess.RemoveAllListeners();
            qteController.onFail.RemoveAllListeners();
            qteController.onSuccess.AddListener(OnSuccess);
            qteController.onFail.AddListener(OnFail);
            qteController.StartQTE();
        }
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

        if (qteController != null)
        {
            qteController.onSuccess.RemoveAllListeners();
            qteController.onFail.RemoveAllListeners();
            qteController.gameObject.SetActive(false);
        }

        // Avisar a la trampa para que libere al jugador
        if (currentTrap != null)
            currentTrap.ReleasePlayer(gameObject);

        SetPlayerLocked(false);

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
        // End the immediate QTE UI interaction.
        isInQTE = false;

        // Hide QTE UI if present
        if (qteUI != null)
            qteUI.SetActive(false);

        if (qteController != null)
        {
            qteController.onSuccess.RemoveAllListeners();
            qteController.onFail.RemoveAllListeners();
            qteController.gameObject.SetActive(false);
        }

        // Reset counters so a future QTE starts clean
        pressCount = 0;
        timer = timeLimit;

        // Mantener al jugador atrapado hasta el Game Over
        if (playerAnimator != null)
            playerAnimator.SetTrigger("AtadoTentaculo");

        SetPlayerLocked(true);

        if (currentTrap != null)
            currentTrap.FailPlayer(gameObject);

        currentTrap = null;
    }

    private void SetPlayerLocked(bool locked)
    {
        if (playerController != null)
            playerController.SetTrapped(locked);

        if (playerMovement != null)
        {
            if (locked)
            {
                playerMovement.SetSpeedMultiplier(0f);
                playerMovement.enabled = false;
            }
            else
            {
                playerMovement.ResetSpeed();
                playerMovement.enabled = true;
            }
        }

        if (playerRb != null)
            playerRb.linearVelocity = Vector2.zero;
    }
}