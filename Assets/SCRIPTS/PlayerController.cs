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
    private bool isMovingGlobal = false;

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
        isMovingGlobal = isMoving;

        animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            float moveX = movement.x;
            if (Mathf.Abs(moveX) < 0.01f)
            {
                moveX = lastDirectionX;
            }

            animator.SetFloat("MoveX", moveX);
            animator.SetFloat("MoveY", movement.y);
            if (Mathf.Abs(movement.x) > 0.01f)
            {
                lastDirectionX = movement.x; // Guardar la dirección horizontal real
                facingLeft = movement.x < 0;
            }
        }
        else
        {
            animator.SetFloat("MoveX", lastDirectionX); // Mantener dirección en idle
            animator.SetFloat("MoveY", 0f);
            facingLeft = lastDirectionX < 0;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = facingLeft;
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
        // El Animator maneja la dirección con MoveX, no necesitamos flip manual
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

    // Alias para SetMovement (usado por BossController durante stun)
    public void SetCanMove(bool value)
    {
        SetMovement(value);
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
