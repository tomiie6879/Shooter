using UnityEngine;

public sealed class EnemyHealth : MonoBehaviour, IHealth, IDamageable
{
    [SerializeField, Min(1f)] private float maxHealth = 30f;

    private PoolItem poolItem;
    private CombatPoolManager dropPool;
    private EnemyData enemyData;
    private bool healthInitialized;

    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }
    public bool IsAlive => CurrentHealth > 0f;

    private void Awake()
    {
        poolItem = GetComponent<PoolItem>();

        // Enemy có thể đã được chuẩn bị khi còn inactive.
        if (!healthInitialized)
        {
            CurrentHealth = maxHealth;
            healthInitialized = true;
        }
    }

    public void ResetHealth(float newMaxHealth)
    {
        maxHealth = Mathf.Max(1f, newMaxHealth);
        CurrentHealth = maxHealth;
        healthInitialized = true;
    }

    public void ConfigureDrops(
        CombatPoolManager pool,
        EnemyData data)
    {
        dropPool = pool;
        enemyData = data;
    }

    public void TakeDamage(float damage)
    {
        if (!IsAlive || damage <= 0f || Time.timeScale <= 0f)
            return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - damage);

        if (!IsAlive)
            Die();
    }

    private void Die()
    {
        if (poolItem == null)
            poolItem = GetComponent<PoolItem>();

        if (poolItem != null && poolItem.IsRented)
        {
            // Thả đồ trước khi Enemy bị ẩn và đổi cha.
            if (dropPool != null && enemyData != null)
                dropPool.DropEnemyLoot(enemyData, transform.position);

            poolItem.ReturnToPool();
            return;
        }

        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        dropPool = null;
        enemyData = null;
    }
}