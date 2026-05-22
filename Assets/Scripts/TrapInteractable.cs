using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class TrapInteractable : MonoBehaviour
{
    private enum TrapState
    {
        Idle,
        Prepared,
        Active,
        Cooldown,
        Used
    }

    [Header("Trampa")]
    public TrapData trapData;

    [Header("Referencias opcionales")]
    public BossHealth bossHealth;
    public Animator animator;
    public ParticleSystem effectParticles;
    public AudioSource audioSource;
    public GameObject preparedVisual;

    [Header("Configuración")]
    public bool debugLogs = true;
    [Tooltip("Etiqueta del boss que activa el trigger.")]
    public string bossTag = "Boss";

    [Header("Unity Events")]
    public UnityEvent onTrapPrepared;
    public UnityEvent onTrapActivated;
    public UnityEvent onTrapCooldown;

    private TrapState currentState = TrapState.Idle;
    private float cooldownTimer = 0f;
    private TrapData.TrapItemRequirement activeRequirement;

    private void Awake()
    {
        if (trapData == null)
            DebugLog("TrapData no asignado en " + name);

        if (animator == null)
            animator = GetComponent<Animator>();

        UpdateVisualState();
    }

    private void Update()
    {
        if (currentState == TrapState.Cooldown && trapData != null && trapData.cooldown > 0f)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= trapData.cooldown)
                EndCooldown();
        }
    }

    public void Interact(GameObject actor)
    {
        if (trapData == null)
        {
            DebugLog("No hay TrapData asignado");
            return;
        }

        if (trapData.trapType == TrapType.PLACEABLE)
        {
            DebugLog("Esta trampa se coloca con PlaceableTrap y no se activa directamente.");
            return;
        }

        if (trapData.trapType == TrapType.STATIC_PREPARED)
        {
            if (currentState == TrapState.Idle)
            {
                if (Prepare(actor))
                    DebugLog("Trampa preparada: " + trapData.trapName);
                else
                    DebugLog("No se pudo preparar la trampa: " + trapData.trapName);
            }
            else
            {
                DebugLog("Trampa ya preparada o en cooldown: " + trapData.trapName);
            }
            return;
        }

        if (trapData.trapType == TrapType.ACTIONABLE)
        {
            if (Activate(actor))
                DebugLog("Trampa activada: " + trapData.trapName);
            else
                DebugLog("No se pudo activar la trampa: " + trapData.trapName);
            return;
        }
    }

    public bool Prepare(GameObject actor)
    {
        if (trapData == null || currentState != TrapState.Idle)
            return false;

        InventoryManager inventory = actor?.GetComponent<InventoryManager>();
        activeRequirement = GetBestRequirement(inventory, requireItems: true);

        if (activeRequirement == null && trapData.requiredItem != null)
        {
            DebugLog("Faltan items requeridos para preparar");
            return false;
        }

        if (activeRequirement != null && trapData.consumeItems)
        {
            if (!ConsumeRequirement(inventory, activeRequirement))
            {
                DebugLog("Error al consumir items para preparar");
                return false;
            }
        }

        SetState(TrapState.Prepared);
        PlayFeedback(trapData.prepareText);
        onTrapPrepared?.Invoke();
        trapData.onPrepared?.Invoke();
        return true;
    }

    public bool Activate(GameObject actor)
    {
        if (trapData == null || currentState == TrapState.Cooldown || currentState == TrapState.Used)
            return false;

        if (trapData.trapType != TrapType.ACTIONABLE)
            return false;

        InventoryManager inventory = actor?.GetComponent<InventoryManager>();
        activeRequirement = activeRequirement ?? GetBestRequirement(inventory, requireItems: true);

        if (activeRequirement == null && trapData.requiredItem != null)
        {
            DebugLog("Faltan items requeridos para activar");
            return false;
        }

        if (activeRequirement != null && trapData.consumeItems)
        {
            if (!ConsumeRequirement(inventory, activeRequirement))
            {
                DebugLog("Error al consumir items para activar");
                return false;
            }
        }

        SetState(TrapState.Active);
        PlayFeedback(trapData.activeText);
        onTrapActivated?.Invoke();
        trapData.onActivated?.Invoke();

        if (trapData.cooldown > 0f)
            StartCooldown();

        if (trapData.destroyAfterUse && !trapData.reusable)
            Destroy(gameObject, 0.15f);

        return true;
    }

    public void TriggerBoss(GameObject boss)
    {
        if (trapData == null || boss == null)
            return;

        if (!boss.CompareTag(bossTag))
            return;

        if (trapData.trapType == TrapType.STATIC_PREPARED && currentState != TrapState.Prepared)
            return;

        if (trapData.trapType == TrapType.ACTIONABLE && currentState != TrapState.Active)
            return;

        if (trapData.trapType == TrapType.PLACEABLE && currentState != TrapState.Prepared && !trapData.autoActivate)
            return;

        BossHealth bossHealthTarget = bossHealth ?? boss.GetComponent<BossHealth>();
        if (bossHealthTarget == null)
        {
            DebugLog("BossHealth no encontrado en el boss");
            return;
        }

        float damage = trapData.damage;
        if (activeRequirement != null && activeRequirement.damage > 0)
            damage = activeRequirement.damage;

        bossHealthTarget.TakeDamage(damage);
        DebugLog($"Boss recibió {damage} de {trapData.trapName}");

        PlayFeedback(trapData.activeText);
        onTrapActivated?.Invoke();
        trapData.onActivated?.Invoke();

        if (trapData.reusable)
        {
            if (trapData.cooldown > 0f)
                StartCooldown();
            else
                SetState(TrapState.Idle);
        }
        else
        {
            SetState(TrapState.Used);
            if (trapData.destroyAfterUse)
                Destroy(gameObject, 0.15f);
        }
    }

    private TrapData.TrapItemRequirement GetBestRequirement(InventoryManager inventory, bool requireItems)
    {
        if (trapData == null)
            return null;

        if (trapData.itemRequirements != null && trapData.itemRequirements.Count > 0 && inventory != null)
        {
            foreach (var requirement in trapData.itemRequirements)
            {
                if (requirement.primaryItem != null && inventory.HasItem(requirement.primaryItem) &&
                    (requirement.secondaryItem == null || inventory.HasItem(requirement.secondaryItem)))
                {
                    DebugLog("Seleccionado requisito: " + requirement.label);
                    return requirement;
                }
            }
        }

        if (trapData.requiredItem != null)
        {
            if (inventory != null && inventory.HasItem(trapData.requiredItem) &&
                (trapData.secondaryItem == null || inventory.HasItem(trapData.secondaryItem)))
            {
                return new TrapData.TrapItemRequirement
                {
                    primaryItem = trapData.requiredItem,
                    secondaryItem = trapData.secondaryItem,
                    damage = trapData.damage,
                    stunDuration = trapData.stunDuration,
                    consumePrimary = trapData.consumeItems,
                    consumeSecondary = trapData.consumeItems
                };
            }

            if (requireItems)
                return null;
        }

        return new TrapData.TrapItemRequirement
        {
            damage = trapData.damage,
            stunDuration = trapData.stunDuration,
            consumePrimary = false,
            consumeSecondary = false
        };
    }

    private bool ConsumeRequirement(InventoryManager inventory, TrapData.TrapItemRequirement requirement)
    {
        if (inventory == null || requirement == null)
            return false;

        if (requirement.primaryItem != null && requirement.consumePrimary)
        {
            if (!inventory.ConsumeItem(requirement.primaryItem))
                return false;
        }

        if (requirement.secondaryItem != null && requirement.consumeSecondary)
        {
            if (!inventory.ConsumeItem(requirement.secondaryItem))
                return false;
        }

        return true;
    }

    private void StartCooldown()
    {
        currentState = TrapState.Cooldown;
        cooldownTimer = 0f;
        onTrapCooldown?.Invoke();
        trapData.onCooldown?.Invoke();
        UpdateVisualState();
    }

    private void EndCooldown()
    {
        currentState = TrapState.Idle;
        cooldownTimer = 0f;
        UpdateVisualState();
    }

    private void SetState(TrapState newState)
    {
        currentState = newState;
        UpdateVisualState();
    }

    private void UpdateVisualState()
    {
        if (preparedVisual != null)
            preparedVisual.SetActive(currentState == TrapState.Prepared || currentState == TrapState.Active);

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            switch (currentState)
            {
                case TrapState.Prepared:
                    sprite.color = new Color(1f, 0.85f, 0.2f, 1f);
                    break;
                case TrapState.Active:
                    sprite.color = new Color(1f, 0.6f, 0.2f, 1f);
                    break;
                case TrapState.Cooldown:
                    sprite.color = new Color(0.7f, 0.7f, 0.7f, 1f);
                    break;
                case TrapState.Used:
                    sprite.color = new Color(0.4f, 0.4f, 0.4f, 1f);
                    break;
                default:
                    sprite.color = Color.white;
                    break;
            }
        }
    }

    private void PlayFeedback(string label)
    {
        if (!string.IsNullOrEmpty(label))
            DebugLog(label);

        PlayAnimation();
        PlayParticles();
        PlayAudio();
    }

    private void PlayAnimation()
    {
        if (animator != null && trapData != null && !string.IsNullOrEmpty(trapData.animatorTrigger))
            animator.SetTrigger(trapData.animatorTrigger);
    }

    private void PlayParticles()
    {
        if (effectParticles != null)
        {
            effectParticles.Play();
            return;
        }

        if (trapData != null && trapData.vfxPrefab != null)
            Instantiate(trapData.vfxPrefab, transform.position, Quaternion.identity);
    }

    private void PlayAudio()
    {
        if (audioSource == null || trapData == null || trapData.audioClips == null || trapData.audioClips.Length == 0)
            return;

        AudioClip clip = trapData.audioClips[Random.Range(0, trapData.audioClips.Length)];
        audioSource.PlayOneShot(clip);
    }

    private void DebugLog(string message)
    {
        if (debugLogs)
            Debug.Log(message);
    }

    public void ForcePrepare()
    {
        if (trapData == null)
            return;

        currentState = TrapState.Prepared;
        UpdateVisualState();
        DebugLog("Trampa forzada a PREPARED: " + trapData.trapName);
    }
}
