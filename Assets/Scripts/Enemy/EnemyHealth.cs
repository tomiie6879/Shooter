using UnityEngine;

public sealed class EnemyHealth : MonoBehaviour, IHealth, IDamageable
{
    [SerializeField, Min(1f)] private float maxHealth = 30f;

    private PoolItem poolItem;

    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }
    public bool IsAlive => CurrentHealth > 0f;

    private void Awake()
    {
        poolItem = GetComponent<PoolItem>();
        CurrentHealth = maxHealth;
    }

    // Gọi mỗi lần lấy enemy ra từ pool.
    public void ResetHealth(float newMaxHealth)
    {
        maxHealth = Mathf.Max(1f, newMaxHealth);
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (!IsAlive || damage <= 0f)
            return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - damage);

        if (!IsAlive)
            Die();
    }

    private void Die()
    {
        if (poolItem != null && poolItem.IsRented)
        {
            poolItem.ReturnToPool();
            return;
        }

        // Hỗ trợ enemy đặt thủ công trong scene, không thuộc pool.
        gameObject.SetActive(false);
    }
}