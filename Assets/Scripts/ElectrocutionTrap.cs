using UnityEngine;
using System.Collections;

public class ElectrocutionTrap : MonoBehaviour
{
    [Header("Detección")]
    public string bossTag = "Boss";
    public float detectionRadius = 2f;
    public Transform detectionCenter;

    [Header("Daño & Stun")]
    public int damage = 2;
    public float stunDuration = 2.5f;
    public string electrocuteAnimationTrigger = "electrocutarse";

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public string activateAnimationTrigger = "Activate";

    private bool isActive = false;
    private bool hasElectrocuted = false;
    private BossController bossInTrap = null;

    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null) animator = GetComponent<Animator>();
        if (detectionCenter == null) detectionCenter = transform;

        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (animator != null) animator.enabled = false;
    }

    void Update()
    {
        if (isActive && !hasElectrocuted)
        {
            DetectBoss();
        }
    }

    public void Activate()
    {
        if (isActive) return;

        isActive = true;
        hasElectrocuted = false;

        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (animator != null) animator.enabled = true;

        if (animator != null && !string.IsNullOrEmpty(activateAnimationTrigger))
        {
            animator.SetTrigger(activateAnimationTrigger);
        }

        Debug.Log("ElectrocutionTrap activada");
    }

    private void DetectBoss()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(detectionCenter.position, detectionRadius);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag(bossTag))
            {
                BossController boss = collider.GetComponent<BossController>();
                if (boss != null)
                {
                    Electrocute(boss);
                    hasElectrocuted = true;
                    return;
                }

                BossHealth bossHealth = collider.GetComponent<BossHealth>();
                if (bossHealth != null && boss == null)
                {
                    boss = collider.GetComponentInParent<BossController>();
                    if (boss != null)
                    {
                        Electrocute(boss);
                        hasElectrocuted = true;
                        return;
                    }
                }
            }
        }
    }

    private void Electrocute(BossController boss)
    {
        bossInTrap = boss;

        boss.RecibirDaño(damage);
        boss.Trap_Stun(stunDuration, electrocuteAnimationTrigger);

        Debug.Log($"ElectrocutionTrap: Jefe electrocutado (-{damage} HP, stunned {stunDuration}s)");
    }

    void OnDrawGizmosSelected()
    {
        if (detectionCenter == null) detectionCenter = transform;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(detectionCenter.position, detectionRadius);
    }
}
