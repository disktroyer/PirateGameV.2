using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    public class TopDownCharacterController : MonoBehaviour
    {
        public float speed = 5f;

        private Animator animator;
        private Rigidbody2D rb;

        private void Start()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            Vector2 dir = Vector2.zero;

            // Movimiento lateral
            if (Input.GetKey(KeyCode.A))
            {
                dir.x = -1;
                animator.SetInteger("Direction", 3); // Izquierda
            }
            else if (Input.GetKey(KeyCode.D))
            {
                dir.x = 1;
                animator.SetInteger("Direction", 2); // Derecha
            }

            // Movimiento vertical
            if (Input.GetKey(KeyCode.W))
            {
                dir.y = 1;
                animator.SetInteger("Direction", 1); // Arriba
            }
            else if (Input.GetKey(KeyCode.S))
            {
                dir.y = -1;
                animator.SetInteger("Direction", 0); // Abajo
            }

            // Normalizar y aplicar movimiento
            dir.Normalize();
            animator.SetBool("IsMoving", dir.magnitude > 0);
        }

        private void FixedUpdate()
        {
            rb.linearVelocity = speed * new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            ).normalized;
            
        }
    }
}
