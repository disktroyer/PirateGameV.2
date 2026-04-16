using UnityEngine;
using UnityEngine.SceneManagement;

public class TentacleTrap : MonoBehaviour
{
    [Header("Trap Settings")]
    public float slowMultiplier = 0f; // 0 = paralizado completamente
    public string activationTag = "Boss";

    [Header("References")]
    public GameObject trapVisual;       // SpriteRenderer de la trampa en el suelo
    public Animator trapAnimator;       // Animator de la propia trampa
    public SpriteRenderer trapSprite;   // Sprite que se desactiva al atrapar al jugador

    private bool isActive = false;
    private bool hasTrappedPlayer = false;

    private PlayerController trappedPlayerController;
    private PlayerMovement trappedPlayerMovement;
    private Rigidbody2D trappedPlayerRb;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Jefe o enemigo activo activa la trampa
        if (other.CompareTag(activationTag) || other.GetComponent<BossController>() != null)
        {
            ActivateTrap();
            return;
        }

        // Jugador cae en la trampa
        if (!other.CompareTag("Player")) return;
        if (!isActive || hasTrappedPlayer) return;

        hasTrappedPlayer = true;

        PlayerQTEController qte = other.GetComponent<PlayerQTEController>();
        PlayerMovement movement = other.GetComponent<PlayerMovement>();
        PlayerController playerController = other.GetComponent<PlayerController>();
        Animator playerAnimator = other.GetComponent<Animator>();
        Rigidbody2D playerRb = other.attachedRigidbody != null ? other.attachedRigidbody : other.GetComponent<Rigidbody2D>();

        trappedPlayerMovement = movement;
        trappedPlayerController = playerController;
        trappedPlayerRb = playerRb;

        // Paralizar al jugador
        LockPlayer(true);

        // Forzar animación de atado en el jugador (mantener hasta liberación)
        if (playerAnimator != null)
            playerAnimator.SetTrigger("AtadoTentaculo");

        // Ocultar el sprite de la trampa: solo se ve el tentáculo atado al jugador
        if (trapSprite != null)
            trapSprite.enabled = false;

        // Animación de la trampa: tentáculo aparece atado
        if (trapAnimator != null)
            trapAnimator.SetTrigger("AtadoTentaculo");

        // Iniciar QTE en el jugador
        if (qte != null)
            qte.StartQTE(this);
    }

    public void ActivateTrap()
    {
        if (isActive) return;
        isActive = true;

        if (trapVisual != null)
            trapVisual.SetActive(true);

        if (trapAnimator != null)
            trapAnimator.SetTrigger("Appear");
    }

    // Llamado por PlayerQTEController cuando el jugador escapa
    public void ReleasePlayer(GameObject player)
    {
        LockPlayer(false);

        // Asegurarse de que la animación del jugador vuelva a estado normal
        Animator playerAnimator = player.GetComponent<Animator>();
        if (playerAnimator != null)
            playerAnimator.SetTrigger("Idle");

        Destroy(gameObject);
    }

    public void FailPlayer(GameObject player)
    {
        // Si el jugador falla los 3 intentos en la trampa, ir a Game Over.
        if (player != null)
            player.SetActive(false);

        SceneManager.LoadScene("GameOver");
    }

    private void LockPlayer(bool trapped)
    {
        if (trappedPlayerController != null)
            trappedPlayerController.SetTrapped(trapped);

        if (trappedPlayerMovement != null)
        {
            if (trapped)
            {
                trappedPlayerMovement.SetSpeedMultiplier(slowMultiplier);
                trappedPlayerMovement.enabled = false;
            }
            else
            {
                trappedPlayerMovement.ResetSpeed();
                trappedPlayerMovement.enabled = true;
            }
        }

        if (trappedPlayerRb != null)
            trappedPlayerRb.linearVelocity = Vector2.zero;
    }
}