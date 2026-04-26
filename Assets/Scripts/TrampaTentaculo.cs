using UnityEngine;
using UnityEngine.SceneManagement;

public class TrampaTentaculo : MonoBehaviour
{
    public QTEController qteController;
    public float trapDuration = 6f;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public Transform trapPoint;

    private PlayerController trappedPlayerController;
    private PlayerMovement trappedPlayerMovement;
    private Rigidbody2D trappedPlayerRb;
    private BossController trappedBoss;
    private bool isActive = false;
    private bool isVisible = false;
    private bool trapResolved = false;
    private bool isBossTrap = false;

    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null) animator = GetComponent<Animator>();

        // Asegurar que está oculto al inicio
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (animator != null) animator.enabled = false;

        if (qteController != null)
            qteController.gameObject.SetActive(false);

        Debug.Log("TrampaTentaculo iniciada (oculta, esperando Enemy)");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (trapResolved) return;

        // Mostrar sprite cuando algo pasa por encima
        if ((other.CompareTag("Boss") || other.CompareTag("Player") || other.GetComponent<BossController>() != null) && !isVisible)
        {
            isVisible = true;
            if (spriteRenderer != null) spriteRenderer.enabled = true;
            if (animator != null) animator.enabled = true;
            Debug.Log("TrampaTentaculo: Sprite visible, detectado " + other.tag);
        }

        // Jefe activando la trampa y dejándola armada para el jugador
        if (other.CompareTag("Boss") || other.GetComponent<BossController>() != null)
        {
            if (!isActive)
            {
                isActive = true;
                Debug.Log("TrampaTentaculo: activada por Boss, esperando jugador");
            }
            return;
        }

        if (other.CompareTag("Player"))
        {
            if (!isActive) return;
            isBossTrap = false;

            animator.SetTrigger("Atrapar");

            if (trapPoint != null)
            {
                other.transform.position = trapPoint.position;
            }


            trappedPlayerController = other.GetComponent<PlayerController>();
            trappedPlayerMovement = other.GetComponent<PlayerMovement>();
            trappedPlayerRb = other.attachedRigidbody != null ? other.attachedRigidbody : other.GetComponent<Rigidbody2D>();

            if (trappedPlayerController != null || trappedPlayerMovement != null)
            {

                Debug.Log("TrampaTentaculo: Jugador capturado, iniciando QTE");
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

        //Destroy(gameObject);
    }

    void ReleaseBoss()
    {
        if (trapResolved)
            return;

        trapResolved = true;

        // Aplicar daño al jefe al escapar
        if (trappedBoss != null)
            trappedBoss.RecibirDaño(10f);

        Destroy(gameObject);
    }

    void FailQTE()
    {
        if (trapResolved || qteController == null)
            return;

        trapResolved = true;

        if (isBossTrap)
        {
            Destroy(gameObject);
            return;
        }

        LockPlayer(false);

        GameObject player = null;
        if (trappedPlayerController != null)
            player = trappedPlayerController.gameObject;
        else if (trappedPlayerRb != null)
            player = trappedPlayerRb.gameObject;

        if (player != null)
            player.SetActive(false);

        SceneManager.LoadScene("GameOver");
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
