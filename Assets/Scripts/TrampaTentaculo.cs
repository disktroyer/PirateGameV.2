using UnityEngine;

public class TrampaTentaculo : MonoBehaviour
{
    public QTEController qteController;
    public float trapDuration = 6f;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    private PlayerController trappedPlayer;
    private bool isActive = false;
    private bool isVisible = false;

    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null) animator = GetComponent<Animator>();

        spriteRenderer.enabled = false;
        animator.enabled = false;
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

            trappedPlayer = other.GetComponent<PlayerController>();

            if (trappedPlayer != null)
            {
                    trappedPlayer.SetTrapped(true);

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
        if (trappedPlayer != null)
        {
            trappedPlayer.SetTrapped(false);
        }

        Destroy(gameObject);
    }

    void FailQTE()
    {
        // Aquí decides qué pasa si falla:
        // opción 1: volver a intentar automáticamente
        qteController.StartQTE();

        // opción 2: daño
        // trappedPlayer.TakeDamage(10);

        // opción 3: tiempo extra atrapado
    }
}
