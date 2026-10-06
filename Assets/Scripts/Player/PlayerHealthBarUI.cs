using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class PlayerHealthBarUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text healthText;

    private void OnEnable()
    {
        if (playerHealth == null)
            return;

        playerHealth.HealthChanged += UpdateHealthBar;
        Refresh();
    }

    private void Start()
    {
        // Đồng bộ sau khi các component đã chạy Awake.
        Refresh();
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.HealthChanged -= UpdateHealthBar;
    }

    private void Refresh()
    {
        if (playerHealth == null)
            return;

        UpdateHealthBar(
            playerHealth.CurrentHealth,
            playerHealth.MaxHealth
        );
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = maxHealth > 0f
                ? Mathf.Clamp01(currentHealth / maxHealth)
                : 0f;
        }

        if (healthText != null)
        {
            healthText.SetText(
                "{0}/{1}",
                Mathf.CeilToInt(currentHealth),
                Mathf.CeilToInt(maxHealth)
            );
        }
    }
}