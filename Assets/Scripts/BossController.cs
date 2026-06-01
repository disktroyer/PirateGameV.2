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

    [Header("Trap Damage")]
    [Range(0f, 1f)]
    public float trapDamageMultiplier = 0.5f;

    [Header("Hearts UI (optional)")]
    public List<Image> heartIcons;
    public bool useHeartUI = true;
    [Min(1)] public int totalHearts = 5;
    public RectTransform heartsContainer;
    public Sprite heartSprite;
    public Vector2 heartSize = new Vector2(36f, 36f);
    public float heartSpacing = 8f;
    public bool hideLegacyHealthUI = true;

    [Header("Animator Triggers")]
    public string stunTrigger = "Stun";
    public string drinkTrigger = "Drink";
    public string slipTrigger = "Slip";

    [Header("Stun Camera & Player Control")]
    public bool enableStunCamera = true; // Si cambiar cámara y congelar jugador durante stun
    public bool freezePlayerDuringStun = false; // Si se debe bloquear al jugador durante el stun
    public bool cameraOnlyDuringStun = true; // Si true, solo la cámara se moverá al jefe durante el stun

    [Header("Muerte y Llave")]
    public GameObject keyPrefab; // Prefab de la llave a spawnear
    public Transform keySpawnPoint; // Punto donde spawnear la llave
    public SpriteRenderer bossSprite; // Para colorear rojo
    public bool isDead = false;

    private BossState state = BossState.Patrol;
    private int currentWaypoint = 0;
    private Transform player;
    private PlayerController playerController;
    public CameraFocusController cameraFocus;
    private CameraFollow cameraFollow;
    private bool isPaused = false;
    private float currentHealth;

    private Vector2 currentTargetPosition;
    private Vector2 lastPosition;
    private float stuckTimer = 0f;
    private bool facingRight = true;
    private bool heartsBuilt = false;

    public Vector2 Direction { get; private set; }

    // -------------------------------------------------
    // INITIALIZATION
    // -------------------------------------------------

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
        EnsureHeartsBuilt();
        UpdateHealthUI();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            playerController = p.GetComponent<PlayerController>();
        }

        cameraFocus = FindObjectOfType<CameraFocusController>();
        cameraFollow = FindObjectOfType<CameraFollow>();

        if (cameraFocus != null && player != null)
        {
            cameraFocus.SetDefaultTarget(player);
        }
    }

    // -------------------------------------------------
    // UPDATE (LOGIC)
    // -------------------------------------------------

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

    // -------------------------------------------------
    // FIXED UPDATE (PHYSICS)
    // -------------------------------------------------

    void FixedUpdate()
    {
        if (isPaused)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 dir = currentTargetPosition - (Vector2)transform.position;

        if (dir.magnitude > 0.01f)
            dir.Normalize();

        Direction = dir;
        rb.linearVelocity = dir * speed;

        // Flip to face direction
        if (Direction.x > 0 && !facingRight)
        {
            facingRight = true;
            transform.localScale = Vector3.one;
        }
        else if (Direction.x < 0 && facingRight)
        {
            facingRight = false;
            transform.localScale = new Vector3(-1, 1, 1);
        }

        DetectStuck();
    }

    // -------------------------------------------------
    // PATROL
    // -------------------------------------------------

    void Patrol()
    {
        if (waypoints == null || waypoints.Count == 0)
        {
            currentTargetPosition = transform.position;
            return;
        }

        Transform target = waypoints[currentWaypoint];
        currentTargetPosition = target.position;

        if (Vector2.Distance(transform.position, target.position) < waypointStopDistance)
        {
            currentWaypoint++;

            if (currentWaypoint >= waypoints.Count)
                currentWaypoint = 0;
        }
    }

    // -------------------------------------------------
    // CHASE
    // -------------------------------------------------

    void Chase()
    {
        if (player == null) return;

        PlayerHideController hide = player.GetComponent<PlayerHideController>();
        if (hide != null && hide.IsHidden)
        {
            currentWaypoint = GetClosestWaypoint();
            state = BossState.Patrol;
            return;
        }

        currentTargetPosition = player.position;
    }

    // -------------------------------------------------
    // PLAYER DETECTION
    // -------------------------------------------------

    void DetectPlayer()
    {
        if (player == null) return;

        PlayerHideController hide = player.GetComponent<PlayerHideController>();
        if (hide != null && hide.IsHidden) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= chaseRange)
            state = BossState.Chase;
    }

    void CheckLosePlayer()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist > loseRange)
        {
            currentWaypoint = GetClosestWaypoint();
            state = BossState.Patrol;
        }
    }

    int GetClosestWaypoint()
    {
        int closestIndex = 0;
        float minDist = Mathf.Infinity;

        for (int i = 0; i < waypoints.Count; i++)
        {
            float dist = Vector2.Distance(transform.position, waypoints[i].position);

            if (dist < minDist)
            {
                minDist = dist;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    // -------------------------------------------------
    // STUCK DETECTION
    // -------------------------------------------------

    void DetectStuck()
    {
        if (Vector2.Distance(transform.position, lastPosition) < 0.01f)
        {
            stuckTimer += Time.fixedDeltaTime;

            if (stuckTimer > 0.8f)
            {
                currentWaypoint = GetClosestWaypoint();
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        lastPosition = transform.position;
    }

    // -------------------------------------------------
    // MUERTE DEL JEFE
    // -------------------------------------------------
    // private void Die()
    // {
    //     isDead = true;

    //     // Rotar 90 grados (tumbarse horizontal)
    //     transform.rotation = Quaternion.Euler(0, 0, 90);
    //     bossSprite.color = Color.red;
       

    //     // Colorear rojo
    //     if (bossSprite != null)
    //     {
    //         bossSprite.color = Color.red;

    //     }

    //     // Spawnear llave
    //     if (keyPrefab != null)
    //     {
    //         Vector3 spawnPos = keySpawnPoint != null ? keySpawnPoint.position : transform.position + Vector3.up;
    //         GameObject llave = Instantiate(keyPrefab, spawnPos, Quaternion.identity);
    //         llave.tag = "Llave";
    //     }

    //     // Deshabilitar movimiento
    //     this.enabled = false;
    //     if (rb != null)
    //     {
    //         rb.linearVelocity = Vector2.zero;
    //         rb.angularVelocity = 0f;
    //         rb.bodyType = RigidbodyType2D.Static;
    //     }
    //     if (animator != null)
    //     {
    //         animator.SetBool("IsMoving 0", false);
    //         animator.SetTrigger("Die");
    //     }
    //     Debug.Log("Jefe derrotado - Llave spawneada");
    // }

  private void Die()
{
    isDead = true;

    // Desactivar colisiones del enemigo
    Collider2D[] colliders = GetComponentsInChildren<Collider2D>();

    foreach (Collider2D col in colliders)
    {
        col.enabled = false;
    }

    // Rotar 90 grados (tumbarse horizontal)
    transform.rotation = Quaternion.Euler(0, 0, 90);

    // Colorear rojo
    if (bossSprite != null)
    {
        bossSprite.color = Color.red;
    }

    // Spawnear llave
    if (keyPrefab != null)
    {
        Vector3 spawnPos = keySpawnPoint != null 
            ? keySpawnPoint.position 
            : transform.position + Vector3.up;

        GameObject llave = Instantiate(keyPrefab, spawnPos, Quaternion.identity);
        llave.tag = "Llave";
    }

    // Deshabilitar movimiento
    this.enabled = false;

    if (rb != null)
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Static;
    }

    if (animator != null)
    {
        animator.SetBool("IsMoving 0", false);
        animator.SetTrigger("Die");
    }

    Debug.Log("Jefe derrotado - Llave spawneada");
}

    // -------------------------------------------------
    // TRAPS / DAMAGE
    // -------------------------------------------------

    public void RecibirDaño(float amount)
    {
        if (isDead) return;
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Trap_Stun(float duration)
    {
        StartCoroutine(TrapPause(duration, stunTrigger));
    }

    public void Trap_Stun(float duration, string customAnimationTrigger)
    {
        StartCoroutine(TrapPause(duration, customAnimationTrigger));
    }

    public void Trap_Drink(float duration, float damage)
    {
        float finalTrapDamage = damage * trapDamageMultiplier;
        RecibirDaño(finalTrapDamage);
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

        // Detener físicamente al jefe y evitar que siga moviéndose.
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        currentTargetPosition = transform.position;
        Direction = Vector2.zero;

        if (animator != null && !string.IsNullOrEmpty(trigger))
            animator.SetTrigger(trigger);

        // Enfocar temporalmente la cámara al jefe durante el stun sin cambiar el target permanente
        if (enableStunCamera)
        {
            if (cameraFocus != null)
            {
                cameraFocus.FocusOn(transform, duration);
            }
            else if (cameraFollow != null)
            {
                cameraFollow.FocusOn(transform, duration);
            }
        }

        // Opcional: congelar jugador durante el stun solo si no estamos en modo "cameraOnlyDuringStun"
        if (!cameraOnlyDuringStun && freezePlayerDuringStun && playerController != null)
        {
            playerController.SetCanMove(false);
        }

        // Si estamos en modo solo cámara, asegurarnos de que el jugador pueda seguir moviéndose
        if (cameraOnlyDuringStun && playerController != null)
        {
            playerController.SetCanMove(true);
        }

        yield return new WaitForSeconds(duration);

        isPaused = false;
        // Si estaba persiguiendo, vuelve a Chase, si no, vuelve a Patrol
        state = prev == BossState.Chase ? BossState.Chase : BossState.Patrol;

        // Si estaba moviéndose antes del stun, fuerza la animación de caminar
        if (animator != null)
        {
            animator.SetBool("IsMoving 0", true);
        }

        // Volver a la normalidad después del stun: el CameraFollow retomará el target original automáticamente
        // (no es necesario llamar a SetTarget aquí cuando usamos FocusOn)

        // Asegurar que el jugador quede habilitado al terminar el stun
        if (playerController != null)
        {
            playerController.SetCanMove(true);
        }
    }

    // -------------------------------------------------
    // ANIMATOR
    // -------------------------------------------------

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

    // -------------------------------------------------
    // UI HEALTH
    // -------------------------------------------------

    void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth;
            if (useHeartUI && hideLegacyHealthUI)
                healthBar.gameObject.SetActive(false);
        }

        if (healthText != null)
        {
            healthText.text = $"Boss: {currentHealth:0}/{maxHealth}";
            if (useHeartUI && hideLegacyHealthUI)
                healthText.gameObject.SetActive(false);
        }

        if (useHeartUI)
            EnsureHeartsBuilt();

        if (useHeartUI && heartIcons != null && heartIcons.Count > 0)
        {
            float healthRatio = currentHealth / maxHealth;
            int visibleHearts = Mathf.CeilToInt(healthRatio * heartIcons.Count);
            visibleHearts = Mathf.Clamp(visibleHearts, 0, heartIcons.Count);

            if (currentHealth <= 0f)
                visibleHearts = 0;

            for (int i = 0; i < heartIcons.Count; i++)
            {
                if (heartIcons[i] != null)
                    heartIcons[i].enabled = i < visibleHearts;
            }
        }
    }

    void EnsureHeartsBuilt()
    {
        if (heartsBuilt)
            return;

        if (heartIcons == null)
            heartIcons = new List<Image>();

        if (heartIcons.Count > 0)
        {
            heartsBuilt = true;
            totalHearts = heartIcons.Count;
            return;
        }

        if (heartsContainer == null || heartSprite == null)
            return;

        for (int i = 0; i < totalHearts; i++)
        {
            GameObject heartObject = new GameObject($"Heart_{i + 1}", typeof(RectTransform), typeof(Image));
            heartObject.transform.SetParent(heartsContainer, false);

            RectTransform rect = heartObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = new Vector2(0f, 0.5f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.sizeDelta = heartSize;
            rect.anchoredPosition = new Vector2(i * (heartSize.x + heartSpacing), 0f);

            Image image = heartObject.GetComponent<Image>();
            image.sprite = heartSprite;
            image.preserveAspect = true;
            heartIcons.Add(image);
        }

        heartsBuilt = true;
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
