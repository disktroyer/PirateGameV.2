using UnityEngine;
using TMPro; // No olvides esta l�nea para TextMeshPro

public class DigitalClock : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI clockText; // Arrastra aqu� tu objeto de texto

    [Header("Time Settings")]
    [Tooltip("Duraci�n total en minutos reales que representar� el d�a de juego.")]
    public float realTimeDurationMinutes = 8.0f;

    [Tooltip("Hora de inicio del d�a en formato 24h (ej: 6 para las 6 AM).")]
    public int startHour = 6;

    [Tooltip("Hora de fin del d�a en formato 24h (ej: 20 para las 8 PM).")]
    public int endHour = 20;

    private float _elapsedGameSeconds; // Segundos de JUEGO que han pasado
    private float _timeScale;          // Multiplicador para acelerar el tiempo
    private bool _isTimeUp = false;    // Para detener el reloj al final

    void Start()
    {
        // 1. Calculamos la duraci�n total en segundos para el juego y el tiempo real
        float gameDayDurationInSeconds = (endHour - startHour) * 3600f;
        float realDayDurationInSeconds = realTimeDurationMinutes * 60f;

        // 2. Calculamos el multiplicador de tiempo.
        // Esto nos dice cu�ntos segundos del juego pasan por cada segundo real.
        if (realDayDurationInSeconds > 0)
        {
            _timeScale = gameDayDurationInSeconds / realDayDurationInSeconds;
        }
        else
        {
            _timeScale = 0; // El tiempo no avanzar� si la duraci�n es 0
        }

        if (clockText == null)
        {
            Debug.LogError("�Error! No has asignado el objeto de texto al script del reloj.");
            this.enabled = false;
        }

        // Mostramos la hora de inicio al empezar
        UpdateClockText(startHour, 0);
    }

    void Update()
    {
        // Si el d�a ya termin�, no hacemos nada m�s
        if (_isTimeUp) return;

        // 3. Incrementamos los segundos de JUEGO transcurridos
        _elapsedGameSeconds += Time.deltaTime * _timeScale;

        // 4. Calculamos la hora y los minutos actuales
        // Sumamos los segundos transcurridos a nuestra hora de inicio
        float totalCurrentGameSeconds = (startHour * 3600) + _elapsedGameSeconds;

        // Convertimos los segundos totales de vuelta a formato HH:MM
        int currentHour = (int)(totalCurrentGameSeconds / 3600);
        int currentMinute = (int)((totalCurrentGameSeconds % 3600) / 60);

        // 5. Comprobamos si hemos llegado a la hora final
        if (currentHour >= endHour)
        {
            _isTimeUp = true;
            // Forzamos el reloj a mostrar la hora final exacta
            UpdateClockText(endHour, 0);
            return; // Salimos para no actualizar m�s
        }

        // Actualizamos el texto del reloj en cada frame
        UpdateClockText(currentHour, currentMinute);
    }

    private void UpdateClockText(int hour, int minute)
    {
        string ampm = "AM";
        int displayHour = hour;

        if (hour >= 12)
        {
            ampm = "PM";
            if (hour > 12)
            {
                displayHour = hour - 12;
            }
        }

        // Formato para que siempre se muestren dos d�gitos (ej: 06:05 PM)
        clockText.text = $"{displayHour:00}:{minute:00} {ampm}";
    }
}