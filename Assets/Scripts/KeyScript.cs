using UnityEngine;

public class KeyScript : MonoBehaviour
{
    [Header("Flotar")]
    public float amplitude = 0.5f; // Altura del balanceo
    public float frequency = 1f;   // Velocidad del balanceo
    public bool enableFloating = true;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (enableFloating)
        {
            // Movimiento sinusoidal en Y
            float newY = startPosition.y + Mathf.Sin(Time.time * frequency) * amplitude;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        }
    }
}