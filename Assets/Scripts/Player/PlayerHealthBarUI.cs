using UnityEngine;
using UnityEngine.UI;

public sealed class PlayerHealthBarUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image fillImage;
    private void OnEnable()
    {
        if(playerHealth != null)
        {
            playerHealth.HealthChanged += UpdateHealthBar;
        }
    }
    private void OnDisable()
    {
        if(playerHealth != null)
        {
            playerHealth.HealthChanged -= UpdateHealthBar;
        }
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if(fillImage == null || maxHealth <= 0f)
        {
            return;
        }
        fillImage.fillAmount = currentHealth / maxHealth;
    }
}
