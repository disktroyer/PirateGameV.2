using UnityEngine;
using System;

public enum GamePhase { Observacion, Ejecucion }

public class GamePhaseManager : MonoBehaviour
{
    public static GamePhaseManager Instance { get; private set; }

    [Header("Fase actual")]
    public GamePhase currentPhase = GamePhase.Observacion;

    [Header("Duraciones (segundos reales)")]
    public float observacionDuracion = 120f; // 2 minutos
    private float timer;

    public event Action<GamePhase> OnPhaseChanged;

    private void Awake()
    {
        // patrón singleton seguro
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        CambiarFase(GamePhase.Observacion);
    }

    private void Update()
    {
        if (currentPhase == GamePhase.Observacion)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                CambiarFase(GamePhase.Ejecucion);
            }
        }
    }

    public void CambiarFase(GamePhase nuevaFase)
    {
        currentPhase = nuevaFase;

        if (nuevaFase == GamePhase.Observacion)
        {
            timer = observacionDuracion;
            Debug.Log("🔍 Fase de observación iniciada");
        }
        else
        {
            Debug.Log("⚙️ Fase de ejecución iniciada");
        }

        OnPhaseChanged?.Invoke(nuevaFase);
    }

    public bool EsObservacion() => currentPhase == GamePhase.Observacion;
    public bool EsEjecucion() => currentPhase == GamePhase.Ejecucion;
}
