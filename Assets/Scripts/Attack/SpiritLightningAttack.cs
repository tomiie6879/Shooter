using UnityEngine;

public sealed class SpiritLightningAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Transform firePoint;
    [SerializeField] private LightningProjectile projectilePrefab;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Attack")]
    [SerializeField, Min(0.1f)] private float range = 6f;
    [SerializeField, Min(0.05f)] private float cooldown = 0.7f;
    [SerializeField, Min(0f)] private float damage = 10f;

    private float nextAttackTime;

    private void Awake()
    {
        if (playerHealth == null)
            playerHealth = GetComponentInParent<PlayerHealth>();

        // Không gán FirePoint thì bắn từ tâm linh hồn.
        if (firePoint == null)
            firePoint = transform;

        if (projectilePrefab == null || playerHealth == null)
        {
            Debug.LogError(
                "SpiritLightningAttack thiếu Projectile Prefab hoặc Player Health.",
                this
            );

            enabled = false;
        }
    }

    private void LateUpdate()
    {
        if (!playerHealth.IsAlive || Time.time < nextAttackTime)
            return;

        Collider2D target = FindNearestEnemy();

        if (target == null)
        {
            nextAttackTime = Time.time + 0.1f;
            return;
        }

        Vector2 origin = firePoint.position;
        Vector2 direction = (Vector2)target.bounds.center - origin;

        if (direction.sqrMagnitude < 0.0001f)
            direction = Vector2.right;

        LightningProjectile projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        projectile.Launch(
            direction,
            damage,
            range,
            enemyLayer
        );

        nextAttackTime = Time.time + cooldown;
    }

    private Collider2D FindNearestEnemy()
    {
        Collider2D nearest = null;
        float nearestDistance = float.PositiveInfinity;
        Vector2 origin = firePoint.position;

        Collider2D[] candidates = Physics2D.OverlapCircleAll(
            origin,
            range,
            enemyLayer
        );

        foreach (Collider2D candidate in candidates)
        {
            EnemyHealth enemy =
                candidate.GetComponentInParent<EnemyHealth>();

            if (enemy == null || !enemy.IsAlive)
                continue;

            float distance = (
                (Vector2)candidate.bounds.center - origin
            ).sqrMagnitude;

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = candidate;
            }
        }

        return nearest;
    }

    private void OnDrawGizmosSelected()
    {
        Transform origin = firePoint != null ? firePoint : transform;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(origin.position, range);
    }
}