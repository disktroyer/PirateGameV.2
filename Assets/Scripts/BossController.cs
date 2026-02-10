using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BossState
{
    Patrol,
    Chase,
    Stunned
}

public class BossController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;
    public List<Transform> waypoints;
    public float waypointStopDistance = 0.1f;

    [Header("Detection")]
    public float chaseRange = 3f;
    public float loseRange = 4.5f;

    [Header("References")]
    public Animator animator;
    public Rigidbody2D rb;

    [Header("Health (optional UI)")]
    public float maxHealth = 100f;
    public Slider healthBar;
    public TextMeshProUGUI healthText;

    [Header("Animator Triggers")]
    public string stunTrigger = "Stun";
    public string drinkTrigger = "Drink";
    public string slipTrigger = "Slip";

    private BossState state = BossState.Patrol;
    private int currentWaypoint = 0;
    private Transform player;
    private bool isPaused = false;
    private float currentHealth;

    public Vector2 Direction { get; private set; }

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;   
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }


    private void FixedUpdate()
    {
            
    }
    void Update()
    {
        if (isPaused) return;

        switch (state)
        {
            case BossState.Patrol:
                Patrol();
                DetectPlayer();
                break;
            case BossState.Chase:
                Chase();
                CheckLosePlayer();
                break;
        }

        UpdateAnimator();
    }

    // ----------------------------------------
    // Waypoints / Patrol
    // ----------------------------------------

    void Patrol()
    {
        if (waypoints == null || waypoints.Count == 0)
        {
            Direction = Vector2.zero;
            return;
        }

        Transform target = waypoints[currentWaypoint];
        MoveTowards(target.position);

        if (Vector2.Distance(transform.position, target.position) < waypointStopDistance)
        {
            currentWaypoint++;
            if (currentWaypoint >= waypoints.Count)
                currentWaypoint = 0;
        }
    }

    // ----------------------------------------
    // Chase
    // ----------------------------------------

    void Chase()
    {
        if (player == null) return;
        MoveTowards(player.position);
    }

    // ----------------------------------------
    // Movement Core
    // ----------------------------------------

    void MoveTowards(Vector3 target)
    {
        Vector2 dir = target - transform.position;

        dir.Normalize();

        Direction = dir;

        rb.linearVelocity = dir * speed;
    }

    void UpdateAnimator()
    {
        bool moving = Direction.magnitude > 0.01f;

        if (animator != null)
        {
            animator.SetBool("IsMoving 0", moving);
            animator.SetFloat("MoveX", Direction.x);
            animator.SetFloat("MoveY", Direction.y);
        }
    }

    // ----------------------------------------
    // Player Detection
    // ----------------------------------------

    void DetectPlayer()
    {
        if (player == null) return;
        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= chaseRange)
        {
            state = BossState.Chase;
        }
    }

    void CheckLosePlayer()
    {
        if (player == null) return;
        float dist = Vector2.Distance(transform.position, player.position);

        if (dist > loseRange)
        {
            state = BossState.Patrol;
        }
    }

    // ----------------------------------------
    // Interaction / API
    // ----------------------------------------

    public void SetPlayer(Transform p)
    {
        player = p;
    }

    public void RecibirDaño(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }

    public void Trap_Stun(float duration)
    {
        StartCoroutine(TrapPause(duration, stunTrigger));
    }

    public void Trap_Drink(float duration, float damage)
    {
        RecibirDaño(damage);
        StartCoroutine(TrapPause(duration, drinkTrigger));
    }

    public void Trap_Slip(float duration)
    {
        StartCoroutine(TrapPause(duration, slipTrigger));
    }

    IEnumerator TrapPause(float duration, string trigger)
    {
        isPaused = true;
        BossState prev = state;
        state = BossState.Stunned;

        if (animator != null && !string.IsNullOrEmpty(trigger))
            animator.SetTrigger(trigger);

        yield return new WaitForSeconds(duration);

        isPaused = false;
        state = prev == BossState.Chase ? BossState.Chase : BossState.Patrol;
    }

    // ----------------------------------------
    // UI Health
    // ----------------------------------------

    void UpdateHealthUI()
    {
        if (healthBar != null)
            healthBar.value = currentHealth / maxHealth;

        if (healthText != null)
            healthText.text = $"Boss: {currentHealth:0}/{maxHealth}";
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Color color = Color.red;
        color.a = 0.5f;
        Gizmos.color = color;

        Gizmos.DrawWireSphere(transform.position, chaseRange);
        color = Color.yellow;
        color.a = 0.5f;
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, loseRange);
    }
#endif
}
