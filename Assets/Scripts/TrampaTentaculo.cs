using UnityEngine;

public class TrampaTentaculo : MonoBehaviour
{
    public QTEController qteController;
    public float trapDuration = 6f;
    public float retryDelay = 0.2f;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    private PlayerController trappedPlayerController;
    private PlayerMovement trappedPlayerMovement;
    private Rigidbody2D trappedPlayerRb;
    private bool isActive = false;
    private bool isVisible = false;
    private bool trapResolved = false;

    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null) animator = GetComponent<Animator>();

        spriteRenderer.enabled = false;
        animator.enabled = false;

        if (qteController != null)
            qteController.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && !isVisible)
        {
            isVisible = true;
            spriteRenderer.enabled = true;
            animator.enabled = true;
            // animator.SetTrigger("Appear");
        }

        if (isActive) return;

        if (other.CompareTag("Player"))
        {
            isActive = true;

            trappedPlayerController = other.GetComponent<PlayerController>();
            trappedPlayerMovement = other.GetComponent<PlayerMovement>();
            trappedPlayerRb = other.attachedRigidbody != null ? other.attachedRigidbody : other.GetComponent<Rigidbody2D>();

            if (trappedPlayerController != null || trappedPlayerMovement != null)
            {
                LockPlayer(true);

                // Configurar eventos dinámicamente
                qteController.onSuccess.RemoveAllListeners();
                qteController.onFail.RemoveAllListeners();

                qteController.onSuccess.AddListener(ReleasePlayer);
                qteController.onFail.AddListener(FailQTE);

                qteController.StartQTE();
            }
        }
    }

    void ReleasePlayer()
    {
        if (trapResolved)
            return;

        trapResolved = true;
        LockPlayer(false);

        Destroy(gameObject);
    }

    void FailQTE()
    {
        if (trapResolved || qteController == null)
            return;

        Invoke(nameof(RestartQTE), retryDelay);
    }

    void RestartQTE()
    {
        if (trapResolved || qteController == null)
            return;

        qteController.StartQTE();
    }

    void LockPlayer(bool trapped)
    {
        if (trappedPlayerController != null)
            trappedPlayerController.SetTrapped(trapped);

        if (trappedPlayerMovement != null)
        {
            if (trapped)
            {
                trappedPlayerMovement.SetSpeedMultiplier(0f);
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
