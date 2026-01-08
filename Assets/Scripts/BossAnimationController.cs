using UnityEngine;

public class BossAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private EnemyPathController pathController;

    void Update()
    {
        if (pathController == null) return;

        Vector2 dir = pathController.Direction;

        // Flip horizontal
        if (Mathf.Abs(dir.x) > 0.01f)
        {
            spriteRenderer.flipX = dir.x < 0;
        }

        // Animaciones básicas
        animator.SetFloat("MoveX", dir.x);
        animator.SetFloat("MoveY", dir.y);
        animator.SetBool("IsMoving", dir.magnitude > 0.1f);
    }
}
