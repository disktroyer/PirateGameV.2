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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive || hasElectrocuted)
            return;

        if (!other.CompareTag(bossTag))
            return;

        BossController boss = other.GetComponent<BossController>();
        if (boss == null)
            boss = other.GetComponentInParent<BossController>();

        if (boss == null)
            return;

        Electrocute(boss);
        hasElectrocuted = true;
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

        // Si el jefe tiene un Animator y existe un trigger, intentar usar la duración
        // de la animación correspondiente como duración del stun. Si no se encuentra,
        // usar el stunDuration definido en este componente.
        float stunTime = stunDuration;
        try
        {
            if (boss != null && boss.animator != null && !string.IsNullOrEmpty(electrocuteAnimationTrigger))
            {
                var rc = boss.animator.runtimeAnimatorController;
                if (rc != null)
                {
                    var clips = rc.animationClips;
                    if (clips != null && clips.Length > 0)
                    {
                        // Buscar un clip que coincida por nombre con el trigger o que lo contenga
                        foreach (var clip in clips)
                        {
                            if (string.Equals(clip.name, electrocuteAnimationTrigger, System.StringComparison.OrdinalIgnoreCase) ||
                                clip.name.IndexOf(electrocuteAnimationTrigger, System.StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                stunTime = clip.length;
                                break;
                            }
                        }
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("ElectrocutionTrap: error al obtener duración de animación del jefe: " + ex.Message);
        }

        boss.Trap_Stun(stunTime, electrocuteAnimationTrigger);

        Debug.Log($"ElectrocutionTrap: Jefe electrocutado (-{damage} HP, stunned {stunTime}s)");
    }

    void OnDrawGizmosSelected()
    {
        if (detectionCenter == null) detectionCenter = transform;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(detectionCenter.position, detectionRadius);
    }
}
