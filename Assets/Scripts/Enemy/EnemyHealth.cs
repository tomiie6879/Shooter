using UnityEngine;

public sealed class EnemyHealth : MonoBehaviour , IHealth , IDamageable
{
    [SerializeField] private float maxHealth = 30f;

    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }
    public bool IsAlive => CurrentHealth > 0f;
    private void Awake()
    {
        CurrentHealth = maxHealth;
    }
    public void TakeDamage(float damage)
    {
        if(!IsAlive || damage <= 0f)
        {
            return;
        }
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0f);
        Debug.Log($"{name} nhận {damage} damage. HP: {CurrentHealth}");
        if (!IsAlive)
        {
            Die();
        }
    }
    private void Die()
    {
        Debug.Log($"{name} đã chết");
        gameObject.SetActive( false );
    }
}
