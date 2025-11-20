using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathController : MonoBehaviour
{
    [Header("Ruta del Boss")]
    [SerializeField] private List<Transform> waypoints;   // Puntos de patrulla
    [SerializeField] private float speed = 2f;            // Velocidad de movimiento
    [SerializeField] private bool loopPath = true;        // Si vuelve al inicio al llegar al final
    [SerializeField] private float stopDistance = 0.1f;     // Distancia para considerar que llegó al punto

    private int currentTargetIndex = 0;
    private bool isPaused = false;      // Pausado por una trampa
    private Vector2 direction;

    public event System.Action onPointReach;

    public Vector2 Direction { get => direction; }

    private void Awake()
    {
        if (waypoints.Count > 0)
        {
            direction = (waypoints[0].position - transform.position).normalized;
        }
    }

    void Update()
    {
        if (isPaused || waypoints.Count == 0) return;

        Transform target = waypoints[currentTargetIndex];

        // Movimiento hacia el siguiente waypoint
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        // Si llega al punto, cambia al siguiente
        if (Vector2.Distance(transform.position, target.position) < stopDistance)
        {
            currentTargetIndex++;
            if (currentTargetIndex >= waypoints.Count)
            {
                if (loopPath) currentTargetIndex = 0;
                else enabled = false;
            }
            direction = (waypoints[currentTargetIndex].position - transform.position).normalized;
            onPointReach?.Invoke();
        }
    }

    // --- Método que pausa al Boss cuando cae en una trampa ---
    public void PausarPorTrampa(float duracion)
    {
        if (!isPaused)
            StartCoroutine(PausaTemporal(duracion));
    }

    private IEnumerator PausaTemporal(float duracion)
    {
        isPaused = true;
        yield return new WaitForSeconds(duracion);
        isPaused = false;
    }

    // --- Gizmos para ver la ruta ---
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
