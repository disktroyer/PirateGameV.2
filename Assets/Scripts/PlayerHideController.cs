using Cainos.PixelArtTopDown_Basic;
using UnityEngine;

public class PlayerHideController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private TopDownCharacterController movementScript;

    // 🔥 ESTA ES LA VARIABLE REAL QUE USAMOS
    private bool isHidden = false;

    // 🔥 ESTA ES LA PROPIEDAD QUE USA EL BOSS
    public bool IsHidden => isHidden;

    private Transform currentHidePoint;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        movementScript = GetComponent<TopDownCharacterController>();
    }

    public void Hide(Transform hidePoint)
    {
        isHidden = true;
        currentHidePoint = hidePoint;

        // Animación
        animator.SetTrigger("Hide");

        // Colocar al jugador dentro del armario
        transform.position = hidePoint.position;

        // Invisibilidad
        spriteRenderer.enabled = false;

        // No moverse
        movementScript.enabled = false;
        rb.linearVelocity = Vector2.zero;

        // No colisiones
        playerCollider.enabled = false;

        Debug.Log("Jugador escondido");
    }

    public void Unhide()
    {
        isHidden = false;

        // Animación salida
        animator.SetTrigger("Unhide");

        // Volver visible
        spriteRenderer.enabled = true;

        // Rehabilitar movimiento y colisiones
        movementScript.enabled = true;
        playerCollider.enabled = true;

        Debug.Log("Jugador salió del escondite");
    }
}
