using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTeleporter : MonoBehaviour
{
    [Header("Destino del teletransporte")]
    public Transform targetPosition;

    [Header("Opciones")]
    public bool allowPlayer = true;
    public bool allowBoss = true;
    public float cooldown = 0.5f; // medio segundo de protecci�n

    [Header("Transición visual")]
    public bool useAppearDisappear = true;
    public bool useAnimatorTransition = true;
    public string disappearTrigger = "Disappear";
    public string appearTrigger = "Appear";
    public float disappearDuration = 0.12f;
    public float appearDuration = 0.12f;
    public bool disableCollidersDuringTeleport = true;

    private static float lastTeleportTime = -1f;
    private static readonly HashSet<int> teleportingActors = new HashSet<int>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (targetPosition == null)
            return;

        // Evitar bucle: si pas� menos de X segundos, no teletransportamos
        if (Time.time - lastTeleportTime < cooldown)
            return;

        if (!CanTeleport(other, out Transform actorTransform))
            return;

        int actorId = actorTransform.gameObject.GetInstanceID();
        if (teleportingActors.Contains(actorId))
            return;

        StartCoroutine(TeleportRoutine(actorTransform));
    }

    private bool CanTeleport(Collider2D other, out Transform actorTransform)
    {
        actorTransform = other.attachedRigidbody != null ? other.attachedRigidbody.transform : other.transform;

        bool isPlayer = other.CompareTag("Player") || (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"));
        bool isBoss = other.CompareTag("Boss") || (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Boss"));

        if (allowPlayer && isPlayer)
            return true;

        if (allowBoss && isBoss)
            return true;

        return false;
    }

    private IEnumerator TeleportRoutine(Transform actorTransform)
    {
        int actorId = actorTransform.gameObject.GetInstanceID();
        teleportingActors.Add(actorId);
        lastTeleportTime = Time.time;

        Rigidbody2D actorRb = actorTransform.GetComponent<Rigidbody2D>();
        PlayerMovement playerMovement = actorTransform.GetComponent<PlayerMovement>();
        PlayerController playerController = actorTransform.GetComponent<PlayerController>();
        BossController bossController = actorTransform.GetComponent<BossController>();

        bool bossWasEnabled = false;

        if (actorRb != null)
            actorRb.linearVelocity = Vector2.zero;

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerController != null)
            playerController.SetMovement(false);

        if (bossController != null)
        {
            bossWasEnabled = bossController.enabled;
            bossController.enabled = false;
        }

        Collider2D[] actorColliders = actorTransform.GetComponentsInChildren<Collider2D>(true);
        if (disableCollidersDuringTeleport)
        {
            for (int i = 0; i < actorColliders.Length; i++)
                actorColliders[i].enabled = false;
        }

        SpriteRenderer[] sprites = actorTransform.GetComponentsInChildren<SpriteRenderer>(true);
        float[] baseAlphas = CaptureSpriteBaseAlphas(sprites);
        Animator[] animators = actorTransform.GetComponentsInChildren<Animator>(true);

        if (useAppearDisappear)
            yield return PlayTransition(animators, sprites, baseAlphas, disappearTrigger, 1f, 0f, disappearDuration);

        actorTransform.position = targetPosition.position;

        if (useAppearDisappear)
            yield return PlayTransition(animators, sprites, baseAlphas, appearTrigger, 0f, 1f, appearDuration);

        if (disableCollidersDuringTeleport)
        {
            for (int i = 0; i < actorColliders.Length; i++)
                actorColliders[i].enabled = true;
        }

        if (bossController != null)
            bossController.enabled = bossWasEnabled;

        if (playerController != null)
            playerController.SetMovement(true);

        if (playerMovement != null)
            playerMovement.enabled = true;

        teleportingActors.Remove(actorId);
    }

    private IEnumerator PlayTransition(
        Animator[] animators,
        SpriteRenderer[] sprites,
        float[] baseAlphas,
        string triggerName,
        float from,
        float to,
        float duration)
    {
        bool playedAnimator = false;

        if (useAnimatorTransition)
            playedAnimator = TriggerAnimators(animators, triggerName);

        if (playedAnimator)
        {
            if (duration > 0f)
                yield return new WaitForSeconds(duration);

            SetSpritesAlpha(sprites, baseAlphas, to);
            yield break;
        }

        yield return FadeSprites(sprites, baseAlphas, from, to, duration);
    }

    private bool TriggerAnimators(Animator[] animators, string triggerName)
    {
        if (animators == null || animators.Length == 0 || string.IsNullOrWhiteSpace(triggerName))
            return false;

        bool triggered = false;
        for (int i = 0; i < animators.Length; i++)
        {
            Animator animator = animators[i];
            if (animator == null || !animator.isActiveAndEnabled)
                continue;

            if (!HasAnimatorTrigger(animator, triggerName))
                continue;

            animator.SetTrigger(triggerName);
            triggered = true;
        }

        return triggered;
    }

    private bool HasAnimatorTrigger(Animator animator, string triggerName)
    {
        AnimatorControllerParameter[] parameters = animator.parameters;
        for (int i = 0; i < parameters.Length; i++)
        {
            AnimatorControllerParameter parameter = parameters[i];
            if (parameter.type == AnimatorControllerParameterType.Trigger && parameter.name == triggerName)
                return true;
        }

        return false;
    }

    private float[] CaptureSpriteBaseAlphas(SpriteRenderer[] sprites)
    {
        if (sprites == null)
            return null;

        float[] baseAlphas = new float[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
            baseAlphas[i] = sprites[i] != null ? sprites[i].color.a : 1f;

        return baseAlphas;
    }

    private IEnumerator FadeSprites(SpriteRenderer[] sprites, float[] baseAlphas, float from, float to, float duration)
    {
        if (sprites == null || sprites.Length == 0)
            yield break;

        if (duration <= 0f)
        {
            SetSpritesAlpha(sprites, baseAlphas, to);
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float alphaFactor = Mathf.Lerp(from, to, t);

            for (int i = 0; i < sprites.Length; i++)
            {
                if (sprites[i] == null)
                    continue;

                Color c = sprites[i].color;
                c.a = baseAlphas != null && i < baseAlphas.Length ? baseAlphas[i] * alphaFactor : alphaFactor;
                sprites[i].color = c;
            }

            yield return null;
        }

        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] == null)
                continue;

            Color c = sprites[i].color;
            c.a = baseAlphas != null && i < baseAlphas.Length ? baseAlphas[i] * to : to;
            sprites[i].color = c;
        }
    }

    private void SetSpritesAlpha(SpriteRenderer[] sprites, float[] baseAlphas, float alphaFactor)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] == null)
                continue;

            Color c = sprites[i].color;
            c.a = baseAlphas != null && i < baseAlphas.Length ? baseAlphas[i] * alphaFactor : alphaFactor;
            sprites[i].color = c;
        }
    }
}
