using System;
using UnityEngine;

public class BossAnimationController : MonoBehaviour
{
    [SerializeField] private Animator bossAnimator;
    [SerializeField] private SpriteRenderer bossSpriteRenderer;
    [SerializeField] private EnemyPathController pathController;

    private void Start()
    {
        pathController.onPointReach += UpdateDirection;
    }

    private void UpdateDirection()
    {
        //bossSpriteRenderer.flipX = pathController.Direction.x < 0;
        bossSpriteRenderer.transform.localScale = new Vector3(
            pathController.Direction.x < 0 ? -1 : 1,
            bossSpriteRenderer.transform.localScale.y,
            bossSpriteRenderer.transform.localScale.z
        );
    }
}
