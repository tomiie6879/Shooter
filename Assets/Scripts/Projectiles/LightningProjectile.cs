using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public sealed class LightningProjectile : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float speed = 12f;
    [SerializeField, Min(0.01f)] private float hitRadius = 0.1f;
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

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        
        line.widthMultiplier = width;
        line.useWorldSpace = true;
        line.positionCount = 7;
        line.enabled = false;
    }

    public void Launch(
        Vector2 fireDirection,
        float attackDamage,
        float maxDistance,
        LayerMask targetLayer)
    {
        direction = fireDirection.normalized;
        damage = attackDamage;
        remainingDistance = maxDistance;
        enemyLayer = targetLayer;
        traveledDistance = 0f;

        if (direction.sqrMagnitude < 0.001f)
        {
            Destroy(gameObject);
            return;
        }

        launched = true;
        line.enabled = true;
        spawnFrame = Time.frameCount;
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

            enemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        transform.position += (Vector3)(direction * step);
        traveledDistance += step;
        remainingDistance -= step;

        if (remainingDistance <= 0f)
        {
            Destroy(gameObject);
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
}