using UnityEngine;

public class TentacleTrap : MonoBehaviour
{
    [Header("Trap Settings")]
    public float slowMultiplier = 0f; // 0 = paralizado completamente
    public string enemyTag = "Enemy";

    [Header("References")]
    public GameObject trapVisual;       // SpriteRenderer de la trampa en el suelo
    public Animator trapAnimator;       // Animator de la propia trampa
    public SpriteRenderer trapSprite;   // Sprite que se desactiva al atrapar al jugador

    private bool isActive = false;
    private bool hasTrappedPlayer = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Enemigo activa la trampa
        if (other.CompareTag(enemyTag))
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
        Animator playerAnimator = other.GetComponent<Animator>();

        // Paralizar al jugador
        if (movement != null)
        {
            movement.SetSpeedMultiplier(slowMultiplier);
            // Deshabilitar el componente para bloquear inputs mientras está atado
            movement.enabled = false;
        }

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
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.ResetSpeed();
            // Reactivar controles al liberar
            movement.enabled = true;
        }

        // Asegurarse de que la animación del jugador vuelva a estado normal
        Animator playerAnimator = player.GetComponent<Animator>();
        if (playerAnimator != null)
            playerAnimator.SetTrigger("Idle");

        Destroy(gameObject);
    }
}