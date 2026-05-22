using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float baseSpeed = 5f;

    [Header("Animation")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    private float speedMultiplier = 1f;
    private Rigidbody2D rb;
    private Vector2 input;
    private float lastDirectionX = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        if (input.x != 0f)
        {
            lastDirectionX = input.x;
        }

        if (animator != null)
        {
            bool isMoving = input.sqrMagnitude > 0.01f;
            animator.SetBool("IsMoving", isMoving);

            if (isMoving)
            {
                animator.SetFloat("MoveX", input.x);
                animator.SetFloat("MoveY", input.y);
            }
            else
            {
                animator.SetFloat("MoveX", lastDirectionX);
                animator.SetFloat("MoveY", 0f);
            }
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = lastDirectionX < 0f;
        }
    }

    private void FixedUpdate()
    {
        Vector2 velocity = input.normalized * baseSpeed * speedMultiplier;
        rb.linearVelocity = velocity;
    }

    // Called by TentacleTrap to slow the player
    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }

    // Restore normal speed
    public void ResetSpeed()
    {
        speedMultiplier = 1f;
    }
}
