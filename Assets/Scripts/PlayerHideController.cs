using Cainos.PixelArtTopDown_Basic;
using UnityEngine;

public class PlayerHideController : MonoBehaviour
{
    private Animator currentClosetanimator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private TopDownCharacterController movementScript;
    private PlayerController playerController;
    private PlayerMovement playerMovement;

    // 🔥 ESTA ES LA VARIABLE REAL QUE USAMOS
    private bool isHidden = false;

    // 🔥 ESTA ES LA PROPIEDAD QUE USA EL BOSS
    public bool IsHidden => isHidden;
    public HideSpot CurrentHideSpot { get; private set; }

    private Transform currentHidePoint;

    void Start()
    {
        //Closetanimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        movementScript = GetComponent<TopDownCharacterController>();
        playerController = GetComponent<PlayerController>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void Hide(Transform hidePoint, HideSpot hideSpot)
    {
        isHidden = true;
        CurrentHideSpot = hideSpot;
        currentHidePoint = hidePoint;

        if (hideSpot != null)
        {
            currentClosetanimator = hideSpot.GetComponent<Animator>();
        }

        // Animación
        if (currentClosetanimator != null) currentClosetanimator.SetTrigger("EnterCloset");

        // Colocar al jugador dentro del armario
        if (hidePoint != null) transform.position = hidePoint.position;

        // Invisibilidad
        if (spriteRenderer != null) spriteRenderer.enabled = false;

        // No moverse
        if (movementScript != null) movementScript.enabled = false;
        if (playerController != null) playerController.SetMovement(false);
        if (playerMovement != null) playerMovement.enabled = false;
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // No colisiones
        if (playerCollider != null) playerCollider.enabled = false;

        Debug.Log("Jugador escondido");
    }

    public void Unhide()
    {
        isHidden = false;
        CurrentHideSpot = null;

        if (currentClosetanimator != null)
        {
            currentClosetanimator.SetTrigger("ExitCloset");
            // Limpiamos la variable ya que salimos del armario
            currentClosetanimator = null;
        }

        // Volver visible
        if (spriteRenderer != null) spriteRenderer.enabled = true;

        // Rehabilitar movimiento y colisiones
        if (movementScript != null) movementScript.enabled = true;
        if (playerController != null) playerController.SetMovement(true);
        if (playerMovement != null) playerMovement.enabled = true;
        if (playerCollider != null) playerCollider.enabled = true;

        Debug.Log("Jugador salió del escondite");
    }
}
