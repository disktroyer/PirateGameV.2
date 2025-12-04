using UnityEngine;

public enum WaypointType
{
    Normal,
    Idle,
    Interaction,
    ControlDirection
}

public class BossWaypointNode : MonoBehaviour
{
    [Header("Tipo de nodo")]
    public WaypointType type = WaypointType.Normal;

    [Header("Idle Settings")]
    public float idleTime = 2f;

    [Header("Animation Settings")]
    public string animationTrigger;

    [Header("Optional Look Direction (solo ControlDirection)")]
    public Vector2 forcedDirection = Vector2.right;

    // Sistema extensible en el futuro (sonidos, efectos, etc.)
}
