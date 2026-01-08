using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossState
{
    Patrol,
    Chasing,
    Paused
}

public class EnemyPathController : MonoBehaviour
{
    [Header("Ruta")]
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private float speed = 2f;
    [SerializeField] private bool loopPath = true;
    [SerializeField] private float stopDistance = 0.1f;

    [Header("Detección del jugador")]
    [SerializeField] private float chaseRange = 3f;
    [SerializeField] private float loseRange = 5f;

    private int currentIndex = 0;
    private Transform player;
    private BossState state = BossState.Patrol;
    private bool isPaused;

    public Vector2 Direction { get; private set; }

    // -------------------------
    // SET PLAYER DESDE SPAWNER
    // -------------------------
    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    void Update()
    {
        if (isPaused || waypoints.Count == 0)
            return;

        CheckPlayerDistance();

        if (state == BossState.Chasing)
            ChasePlayer();
        else
            Patrol();
    }

    // -------------------------
    // PATRULLA
    // -------------------------
    void Patrol()
    {
        Transform target = waypoints[currentIndex];

        MoveTowards(target.position);

        if (Vector2.Distance(transform.position, target.position) < stopDistance)
        {
            currentIndex++;

            if (currentIndex >= waypoints.Count)
            {
                if (loopPath)
                    currentIndex = 0;
                else
                    currentIndex = waypoints.Count - 1;
            }
        }
    }

    // -------------------------
    // PERSECUCIÓN
    // -------------------------
    void ChasePlayer()
    {
        if (player == null) return;

        MoveTowards(player.position);
    }

    // -------------------------
    // MOVIMIENTO COMÚN
    // -------------------------
    void MoveTowards(Vector3 target)
    {
        Vector2 dir = (target - transform.position).normalized;
        Direction = dir;

        transform.position = Vector2.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );
    }

    // -------------------------
    // DETECCIÓN JUGADOR
    // -------------------------
    void CheckPlayerDistance()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= chaseRange)
            state = BossState.Chasing;
        else if (state == BossState.Chasing && dist > loseRange)
            state = BossState.Patrol;
    }

    // -------------------------
    // PAUSA POR TRAMPA
    // -------------------------
    public void PausarPorTrampa(float time)
    {
        if (!isPaused)
            StartCoroutine(PauseRoutine(time));
    }

    IEnumerator PauseRoutine(float time)
    {
        isPaused = true;
        state = BossState.Paused;
        yield return new WaitForSeconds(time);
        isPaused = false;
        state = BossState.Patrol;
    }

    // -------------------------
    // GIZMOS
    // -------------------------
    void OnDrawGizmosSelected()
    {
        if (waypoints == null || waypoints.Count < 2) return;

        Gizmos.color = Color.red;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
    }
}
