using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(PoolItem))]
public sealed class LightningProjectile : MonoBehaviour
{
    private float speed;
    private float hitRadius;
    [SerializeField, Min(0.01f)] private float boltLength = 0.5f;
    [SerializeField, Min(0f)] private float zigzag = 0.07f;
    [Header("Visual")]
    [SerializeField, Min(0.01f)] private float width = 2f;

    private LineRenderer line;
    private Vector2 direction;
    private LayerMask enemyLayer;
    private int spawnFrame;
    private float damage;
    private float remainingDistance;
    private float traveledDistance;
    private bool launched;
    private PoolItem poolItem;
    private ProjectileData projectileData;
    private CombatPoolManager poolManager;

    private void Awake()
    {
        CacheComponents();
    }

    private void CacheComponents()
    {
        if (line == null)
            line = GetComponent<LineRenderer>();

        if (poolItem == null)
            poolItem = GetComponent<PoolItem>();

        line.widthMultiplier = width;
        line.useWorldSpace = true;
        line.positionCount = 7;
    }

    public void Launch(
    ProjectileData data,
    Vector2 fireDirection,
    float attackDamage,
    LayerMask targetLayer,
    CombatPoolManager manager)
    {
        CacheComponents();

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
        line.enabled = true;
        DrawBolt();
    }

    private void Update()
    {
        if (!launched)
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

        DrawBolt();
    }

    private void DrawBolt()
    {
        // transform.position là đầu gây sát thương.
        Vector3 head = transform.position;
        Vector3 side = new Vector3(-direction.y, direction.x, 0f);

        // Hiện sẵn một đoạn ngắn từ điểm phát ngay khi sinh.
        float initialLength = Mathf.Min(0.15f, boltLength);
        float length = Mathf.Min(
            boltLength,
            initialLength + traveledDistance
        );

        // Phần đầu hiển thị nhô ra nhẹ phía trước đầu va chạm.
        Vector3 visualHead =
            head + (Vector3)direction * initialLength;

        for (int i = 0; i < line.positionCount; i++)
        {
            float t = i / (float)(line.positionCount - 1);

            Vector3 point =
                visualHead - (Vector3)direction * length * t;

            if (i > 0 && i < line.positionCount - 1)
            {
                point += side * Random.Range(-zigzag, zigzag)
                       * Mathf.Clamp01(length / boltLength);
            }

            line.SetPosition(i, point);
        }
    }
    private void OnValidate()
    {
        LineRenderer renderer = GetComponent<LineRenderer>();

        if (renderer != null)
            renderer.widthMultiplier = width;
    }
    private void OnDisable()
    {
        launched = false;
        damage = 0f;
        remainingDistance = 0f;
        traveledDistance = 0f;
        direction = Vector2.zero;

        projectileData = null;
        poolManager = null;

        if (line != null)
            line.enabled = false;
    }
}