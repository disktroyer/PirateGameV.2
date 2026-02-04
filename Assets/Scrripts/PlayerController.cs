using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 6f;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize();

        bool isMoving = movement.magnitude > 0.01f;
        animator.SetBool("IsMoving", isMoving);
        animator.SetFloat("MoveX", movement.x);
        animator.SetFloat("MoveY", movement.y);

        if (isMoving)
        {
            animator.SetFloat("MoveX", movement.x);
            animator.SetFloat("MoveY", movement.y);
        }

        // Movimiento existente...

        if (Input.GetKeyDown(KeyCode.Q))
        {
            // GetComponent<InventoryManager>().DropFirstItem();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            // // Ejemplo: si tienes “Botella vacía” y “Veneno”
            // var inv = GetComponent<InventoryManager>();
            // if (inv.HasItem("Botella Vacia") && inv.HasItem("Veneno"))
            // {
            //     Debug.Log("Crafteado: Veneno listo!");
            //     // Aquí harías AddItem de un item nuevo
            // }
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }
}
