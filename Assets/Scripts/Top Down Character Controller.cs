using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
   
   
   
   
    public class TopDownCharacterController : MonoBehaviour
    {
        public float speed = 3f;

        private Animator animator;
        private Rigidbody2D rb;

        private void Start()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
        }

        //  public void SetSpeedMultiplier(float multiplier)
        // {
        //     currentSpeed = baseSpeed * multiplier;
        // }

        // public void ResetSpeed()
        // {
        //     currentSpeed = baseSpeed;
        // }

        private void Update()
        {
            Vector2 dir = Vector2.zero;

            // --- Movimiento Horizontal ---
            if (Input.GetKey(KeyCode.A))     // izquierda
            {
                dir.x = -1;
                animator.SetInteger("Direction", 3);
                transform.localScale = new Vector3(-1, 1, 1); // mirar a la izq
            }
            else if (Input.GetKey(KeyCode.D)) // derecha
            {
                dir.x = 1;
                animator.SetInteger("Direction", 2);
                transform.localScale = new Vector3(1, 1, 1); // mirar a la der
            }

            // --- Movimiento Vertical ---
            if (Input.GetKey(KeyCode.W))     // arriba
            {
                dir.y = 1;
                animator.SetInteger("Direction", 1);
            }
            else if (Input.GetKey(KeyCode.S)) // abajo
            {
                dir.y = -1;
                animator.SetInteger("Direction", 0);
            }

            // --- Activar animaci�n ---
            dir.Normalize();
            animator.SetBool("IsMoving", dir.magnitude > 0);

            // --- Movimiento real ---
            rb.linearVelocity = speed * dir;
        }

        

        
    }
}
