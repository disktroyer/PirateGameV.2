using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealth : MonoBehaviour
{
    [Header("UI Referencias")]
    public Slider healthBar;
    public TextMeshProUGUI healthText;

    [Header("Corazones (opcional)")]
    public bool useHeartUI = true;
    [Min(1)] public int totalHearts = 5;
    public List<Image> heartIcons;
    public RectTransform heartsContainer;
    public Sprite heartSprite;
    public Vector2 heartSize = new Vector2(36f, 36f);
    public float heartSpacing = 8f;
    public bool hideLegacyHealthUI = true;

    [Header("Muerte")]
    public GameObject keyPrefab; // Prefab de la llave a spawnear
    public Transform keySpawnPoint; // Punto donde spawnear la llave
    public SpriteRenderer bossSprite; // Para colorear rojo
    public bool isDead = false;

    [Header("Datos del Jefe")]
    public float maxHealth = 100f;
    private float currentHealth;
    private bool heartsBuilt = false;

    void Start()
    {
        currentHealth = maxHealth;
        EnsureHeartsBuilt();
        ActualizarUI();
    }

    public void RecibirDaño(float daño)
    {
        if (isDead) return;

        currentHealth -= daño;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        ActualizarUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void ActualizarUI()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth;
            if (useHeartUI && hideLegacyHealthUI)
                healthBar.gameObject.SetActive(false);
        }

        if (healthText != null)
        {
            healthText.text = $"Jefe: {currentHealth:0}/{maxHealth}";
            if (useHeartUI && hideLegacyHealthUI)
                healthText.gameObject.SetActive(false);
        }

        if (useHeartUI)
            EnsureHeartsBuilt();

        if (useHeartUI && heartIcons != null && heartIcons.Count > 0)
        {
            float healthRatio = currentHealth / maxHealth;
            int visibleHearts = Mathf.CeilToInt(healthRatio * heartIcons.Count);
            visibleHearts = Mathf.Clamp(visibleHearts, 0, heartIcons.Count);

            if (currentHealth <= 0f)
                visibleHearts = 0;

            for (int i = 0; i < heartIcons.Count; i++)
            {
                if (heartIcons[i] != null)
                    heartIcons[i].enabled = i < visibleHearts;
            }
        }
    }

    private void EnsureHeartsBuilt()
    {
        if (heartsBuilt)
            return;

        if (heartIcons == null)
            heartIcons = new List<Image>();

        if (heartIcons.Count > 0)
        {
            heartsBuilt = true;
            totalHearts = heartIcons.Count;
            return;
        }

        if (heartsContainer == null || heartSprite == null)
            return;

        for (int i = 0; i < totalHearts; i++)
        {
            GameObject heartObject = new GameObject($"Heart_{i + 1}", typeof(RectTransform), typeof(Image));
            heartObject.transform.SetParent(heartsContainer, false);

            RectTransform rect = heartObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = new Vector2(0f, 0.5f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.sizeDelta = heartSize;
            rect.anchoredPosition = new Vector2(i * (heartSize.x + heartSpacing), 0f);

            Image image = heartObject.GetComponent<Image>();
            image.sprite = heartSprite;
            image.preserveAspect = true;
            heartIcons.Add(image);
        }

        heartsBuilt = true;
    }

    public bool EstaMuerto()
    {
        return currentHealth <= 0;
    }

    private void Die()
    {
        isDead = true;

        // Rotar 90 grados (tumbarse horizontal)
        transform.rotation = Quaternion.Euler(0, 0, 90);

        // Colorear rojo
        if (bossSprite != null)
        {
            bossSprite.color = Color.red;
        }

        // Spawnear llave
        if (keyPrefab != null)
        {
            Vector3 spawnPos = keySpawnPoint != null ? keySpawnPoint.position : transform.position + Vector3.up;
            Instantiate(keyPrefab, spawnPos, Quaternion.identity);
        }

        // Deshabilitar movimiento (si hay BossController)
        var bossController = GetComponent<BossController>();
        if (bossController != null)
        {
            bossController.enabled = false;
        }

        Debug.Log("Jefe derrotado - Llave spawneada");
    }
}
