using UnityEngine;

[RequireComponent(typeof(PoolItem))]
public abstract class PooledProjectile : MonoBehaviour
{
    private float speed;
    private float hitRadius;
    protected Vector2 direction;
    private LayerMask enemyLayer;
    private int spawnFrame;
    private float damage;
    private float remainingDistance;
    protected float traveledDistance;
    private bool launched;
    private PoolItem poolItem;
    private ProjectileData projectileData;
    private CombatPoolManager poolManager;

    public void Launch(
    ProjectileData data,
    Vector2 fireDirection,
    float attackDamage,
    LayerMask targetLayer,
    CombatPoolManager manager)
    {
        if (poolItem == null) poolItem = GetComponent<PoolItem>();

        projectileData = data;
        poolManager = manager;

        direction = fireDirection.normalized;
        damage = attackDamage;
        enemyLayer = targetLayer;

        speed = Mathf.Max(0.1f, data.speed);
        hitRadius = Mathf.Max(0.01f, data.hitRadius);
        remainingDistance = Mathf.Max(0.1f, data.maxDistance);
        traveledDistance = 0f;
        spawnFrame = Time.frameCount;

        if (direction.sqrMagnitude < 0.001f)
        {
            poolItem.ReturnToPool();
            return;
        }

        launched = true;

        // Đã gán xong dữ liệu rồi mới bật object.
        gameObject.SetActive(true);
        OnLaunched();
        DrawVisual();
    }

    protected virtual void Update()
    {
        if (!launched || Time.timeScale <= 0f)
            return;
        // Giữ hình tia sét tại điểm phát trong frame đầu.
        if (Time.frameCount == spawnFrame)
            return;

        float step = Mathf.Min(
            speed * Time.deltaTime,
            remainingDistance
        );

        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            transform.position,
            hitRadius,
            direction,
            step,
            enemyLayer
        );

        foreach (RaycastHit2D hit in hits)
        {
            EnemyHealth enemy =
                hit.collider.GetComponentInParent<EnemyHealth>();

            if (enemy == null || !enemy.IsAlive)
                continue;

            // Chặn xử lý va chạm lần thứ hai.
            launched = false;

            // Lấy tâm trước khi gây damage vì enemy có thể chết và bị tắt.
            Vector3 effectPosition = hit.collider.bounds.center;
            effectPosition.z = transform.position.z;

            enemy.TakeDamage(damage);

            poolManager.PlayVfx(
                projectileData.hitVfx,
                effectPosition
            );

            poolItem.ReturnToPool();
            return;
        }

        transform.position += (Vector3)(direction * step);
        traveledDistance += step;
        remainingDistance -= step;

        if (remainingDistance <= 0f)
        {
            poolItem.ReturnToPool();
            return;
        }

        DrawVisual();
    }

    protected virtual void OnLaunched() {}
    protected virtual void DrawVisual() {}
    protected virtual void ResetVisual() {}
    protected virtual void OnDisable()
    {
        launched = false;
        damage = 0;
        remainingDistance = traveledDistance = 0;
        direction = Vector2.zero;
        projectileData = null;
        poolManager = null;
        ResetVisual();
    }
}
