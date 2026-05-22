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
        private float lastDirectionX = 1f;

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

            // Use input axes so arrows or A/D both work
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            dir.x = h;
            dir.y = v;

            // Preserve last horizontal direction when pressing only vertical
            if (Mathf.Abs(h) > 0.01f)
            {
                lastDirectionX = h;
            }

            // Decide animation Direction with priority: horizontal over vertical
            if (Mathf.Abs(h) > 0.01f)
            {
                if (h < 0)
                    animator.SetInteger("Direction", 3); // left
                else
                    animator.SetInteger("Direction", 2); // right
            }
            else if (Mathf.Abs(v) > 0.01f)
            {
                if (v > 0)
                    animator.SetInteger("Direction", 1); // up
                else
                    animator.SetInteger("Direction", 0); // down
            }

            // Flip sprite according to last horizontal direction
            float scaleX = lastDirectionX < 0f ? -1f : 1f;
            transform.localScale = new Vector3(scaleX, 1, 1);

            // --- Activar animaci�n ---
            dir.Normalize();
            animator.SetBool("IsMoving", dir.magnitude > 0);

            // --- Movimiento real ---
            rb.linearVelocity = speed * dir;
        }

        

        
    }
}
