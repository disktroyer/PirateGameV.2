using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathController : MonoBehaviour
{
    [Header("Ruta del Boss")]
    public List<Transform> waypoints;   // Puntos de patrulla
    public float speed = 2f;            // Velocidad de movimiento
    public bool loopPath = true;        // Si vuelve al inicio al llegar al final

    private int currentTargetIndex = 0;
    private bool isPaused = false;      // Pausado por una trampa

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
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            currentTargetIndex++;
            if (currentTargetIndex >= waypoints.Count)
            {
                if (loopPath) currentTargetIndex = 0;
                else enabled = false;
            }
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
