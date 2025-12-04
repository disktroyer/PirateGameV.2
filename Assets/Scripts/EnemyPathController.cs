using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossState
{
    Patrol,
    Chasing,
    Idle,
    Paused
}

public class EnemyPathController : MonoBehaviour
{
    [Header("Ruta del Boss")]
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private float speed = 2f;
    [SerializeField] private bool loopPath = true;
    [SerializeField] private float stopDistance = 0.1f;

    [Header("Detección del jugador")]
    [SerializeField] private float chaseRange = 3f;
    [SerializeField] private float loseRange = 4.5f;

    private int currentTargetIndex = 0;
    private bool isPaused = false;
    private Vector2 direction;

    private Transform player;
    private BossState state = BossState.Patrol;

    public event System.Action onPointReach;
    public Vector2 Direction => direction;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (waypoints.Count > 0)
            direction = (waypoints[0].position - transform.position).normalized;
    }

    void Update()
    {
        if (isPaused) return;

        CheckPlayerDetection();

        switch (state)
        {
            case BossState.Patrol:
                PatrolMovement();
                break;

            case BossState.Chasing:
                ChasePlayer();
                break;

            case BossState.Idle:
                // No moverse por animación o nodo Idle
                break;

            case BossState.Paused:
                // Controlado por trampa
                break;
        }
    }

    // ---------------------------------------------------------
    // MOVERSE POR WAYPOINTS
    // ---------------------------------------------------------
    void PatrolMovement()
    {
        if (waypoints.Count == 0) return;

        Transform target = waypoints[currentTargetIndex];

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        // ¿Llegó al waypoint?
        if (Vector2.Distance(transform.position, target.position) < stopDistance)
        {
            // Revisamos si el waypoint tiene nodo
            BossWaypointNode node = target.GetComponent<BossWaypointNode>();
            if (node != null)
                HandleWaypointNode(node);

            // Cambiar al siguiente waypoint
            currentTargetIndex++;

            if (currentTargetIndex >= waypoints.Count)
            {
                if (loopPath) currentTargetIndex = 0;
                else
                {
                    enabled = false;
                    return;
                }
            }

            direction = (waypoints[currentTargetIndex].position - transform.position).normalized;
            onPointReach?.Invoke();
        }
    }

    // ---------------------------------------------------------
    // MANEJO DE NODOS ESPECIALES
    // ---------------------------------------------------------
    private void HandleWaypointNode(BossWaypointNode node)
    {
        switch (node.type)
        {
            case WaypointType.Idle:
                StartCoroutine(IdleCoroutine(node.idleTime));
                break;

            case WaypointType.Interaction:
                Animator anim = GetComponentInChildren<Animator>();
                if (anim != null && !string.IsNullOrEmpty(node.animationTrigger))
                    anim.SetTrigger(node.animationTrigger);
                break;

            case WaypointType.ControlDirection:
                direction = node.forcedDirection.normalized;
                break;
        }
    }

    private IEnumerator IdleCoroutine(float time)
    {
        bool oldPaused = isPaused;
        isPaused = true;
        yield return new WaitForSeconds(time);
        isPaused = oldPaused;
    }

    // ---------------------------------------------------------
    // PERSEGUIR JUGADOR
    // ---------------------------------------------------------
    void ChasePlayer()
    {
        if (player == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            speed * Time.deltaTime
        );

        direction = (player.position - transform.position).normalized;
    }

    // ---------------------------------------------------------
    // DETECTAR AL JUGADOR
    // ---------------------------------------------------------
    void CheckPlayerDetection()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= chaseRange)
        {
            state = BossState.Chasing;
        }
        else if (state == BossState.Chasing && dist > loseRange)
        {
            state = BossState.Patrol;
        }
    }

    // ---------------------------------------------------------
    // PAUSA POR TRAMPA
    // ---------------------------------------------------------
    public void PausarPorTrampa(float duracion)
    {
        if (!isPaused)
            StartCoroutine(PausaTemporal(duracion));
    }

    private IEnumerator PausaTemporal(float duracion)
    {
        isPaused = true;
        BossState prev = state;
        state = BossState.Paused;

        yield return new WaitForSeconds(duracion);

        isPaused = false;
        state = BossState.Patrol;
    }

    // ---------------------------------------------------------
    private void OnDrawGizmosSelected()
    {
        if (waypoints == null || waypoints.Count < 2) return;

        Gizmos.color = Color.red;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            Gizmos.DrawSphere(waypoints[i].position, 0.1f);
        }

        if (loopPath)
            Gizmos.DrawLine(waypoints[^1].position, waypoints[0].position);
    }
}
