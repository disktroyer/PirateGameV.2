using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float baseSpeed = 5f;

    private float speedMultiplier = 1f;
    private Rigidbody2D rb;
    private Vector2 input;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
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
