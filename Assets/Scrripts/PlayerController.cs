using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 6f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;

    private bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
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
        }

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
}
