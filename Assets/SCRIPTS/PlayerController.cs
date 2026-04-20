using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 6f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 movement;

    private bool canMove = true;
    private bool isTrapped = false;
    private float lastDirectionX = 1f; // Dirección por defecto a la derecha
    private bool facingLeft = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        // 🔒 Si está atrapado, no leer input
        if (!canMove)
        {
            movement = Vector2.zero;
            animator.SetBool("IsMoving", false);
            return;
        }

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize();

        bool isMoving = movement.magnitude > 0.01f;

        animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            animator.SetFloat("MoveX", movement.x);
            animator.SetFloat("MoveY", movement.y);
            lastDirectionX = movement.x; // Guardar la dirección
            facingLeft = movement.x < 0;
        }
        else
        {
            facingLeft = lastDirectionX < 0;
        }

        //if (Input.GetKey(KeyCode.F))
        //{
        //    animator.SetBool("IsCrafting", true);
        //}
        //else
        //{
        //    animator.SetBool("IsCrafting", false);
        //}

        // Inputs secundarios
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Drop item
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            // Craft
        }
    }

    void LateUpdate()
    {
        UpdateFlip();
    }

    void UpdateFlip()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = facingLeft;
        }
    }

    void FixedUpdate()
    {
        if (!canMove) return;

        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    // -------------------------------------------------
    // API PARA TRAMPAS / QTE
    // -------------------------------------------------

    public void SetMovement(bool value)
    {
        canMove = value;

        if (!canMove)
        {
            movement = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void SetTrapped(bool trapped)
    {
        isTrapped = trapped;
        canMove = !trapped;
        animator.SetBool("IsTrapped", trapped);

        if (trapped)
        {
            movement = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
        }
    }
}
