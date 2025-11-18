using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    public class TopDownCharacterController : MonoBehaviour
    {
        public float speed;

        private Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            Vector2 dir = Vector2.zero;

            // Movimiento horizontal
            if (Input.GetKey(KeyCode.A))
            {
                dir.x = -1;
                animator.SetInteger("Direction", 3);   // Izquierda
            }
            else if (Input.GetKey(KeyCode.D))
            {
                dir.x = 1;
                animator.SetInteger("Direction", 2);   // Derecha
            }

            // Movimiento vertical
            if (Input.GetKey(KeyCode.W))
            {
                dir.y = 1;
                animator.SetInteger("Direction", 1);   // Arriba
            }
            else if (Input.GetKey(KeyCode.S))
            {
                dir.y = -1;
                animator.SetInteger("Direction", 0);   // Abajo
            }

            dir.Normalize();

            animator.SetBool("IsMoving", dir.magnitude > 0);

            // Movimiento real del Rigidbody2D
            GetComponent<Rigidbody2D>().linearVelocity = speed * dir;
        }
    }
}
