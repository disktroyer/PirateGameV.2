using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealth : MonoBehaviour
{
    [Header("UI Referencias")]
    public Slider healthBar;
    public TextMeshProUGUI healthText;

    [Header("Datos del Jefe")]
    public float maxHealth = 100f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        ActualizarUI();
    }

    public void RecibirDaño(float daño)
    {
        currentHealth -= daño;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        ActualizarUI();
    }

    void ActualizarUI()
    {
        if (healthBar != null)
            healthBar.value = currentHealth / maxHealth;
        if (healthText != null)
            healthText.text = $"Jefe: {currentHealth:0}/{maxHealth}";
    }

    public bool EstaMuerto()
    {
        return currentHealth <= 0;
    }
}
