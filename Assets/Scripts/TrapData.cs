using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "TrapData", menuName = "Trampas/TrapData")]
public class TrapData : ScriptableObject
{
    [Header("Identidad")]
    public string trapName;
    [TextArea, Tooltip("Descripción breve de la trampa para diseñadores.")]
    public string description;
    public Sprite icon;
    public TrapType trapType = TrapType.STATIC_PREPARED;

    [Header("Daño y comportamiento")]
    public float damage = 1f;
    public float stunDuration = 0f;
    public bool reusable = false;
    public bool needsPreparation = false;
    public bool autoActivate = false;
    [Tooltip("Si se destruye después de activarse.")]
    public bool destroyAfterUse = true;
    [Tooltip("Tiempo de reutilización después de activarse.")]
    public float cooldown = 0f;

    [Header("Requisitos de items")]
    public ItemData requiredItem;
    public ItemData secondaryItem;
    [Tooltip("Combinaciones especiales de items, por ejemplo bala + pólvora XXL.")]
    public List<TrapItemRequirement> itemRequirements = new List<TrapItemRequirement>();
    public bool consumeItems = true;

    [Header("Efectos")]
    public GameObject vfxPrefab;
    public AudioClip[] audioClips;
    public string animatorTrigger = "Activate";

    [Header("Textos de diseñador")]
    public string interactionText = "Pulsa E para interactuar";
    public string prepareText = "Usa el item requerido para preparar la trampa";
    public string activeText = "Trampa activada";

    [Header("Eventos")]
    public UnityEvent onPrepared;
    public UnityEvent onActivated;
    public UnityEvent onCooldown;

    [System.Serializable]
    public class TrapItemRequirement
    {
        public string label;
        public ItemData primaryItem;
        public ItemData secondaryItem;
        public float damage = 1f;
        public float stunDuration = 0f;
        public bool consumePrimary = true;
        public bool consumeSecondary = true;
    }
}
