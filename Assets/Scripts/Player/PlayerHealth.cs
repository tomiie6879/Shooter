using System;
using UnityEngine;

public sealed class PlayerHealth : MonoBehaviour, IDamageable , IHealth
{
    [SerializeField] private float maxHealth = 100f;
    private PlayerController playerController;
    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }
    public bool IsAlive => CurrentHealth > 0f;

    public event Action<float, float> HealthChanged;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        CurrentHealth = maxHealth;
    }

    private void Start()
    {
        NotifyHealthChanged();
    }
    public void TakeDamage(float damage)
    {
        if(!IsAlive || damage <= 0f)
        {
            return;
        }
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0f);

        NotifyHealthChanged();
        if (!IsAlive)
        {
            playerController?.Die();
        }
    }
    public void Heal(float amount)
    {
        if(!IsAlive || amount <= 0f)
        {
            return;
        }

        CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
        NotifyHealthChanged();
    }
    private void NotifyHealthChanged()
    {
        HealthChanged?.Invoke(CurrentHealth, maxHealth);
    }
}