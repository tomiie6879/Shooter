using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyHealth))]
public sealed class EnemyChase : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth target;
    [SerializeField] private Animator animator;

    [Header("Movement")]
    [SerializeField, Min(0f)] private float moveSpeed = 2f;
    [SerializeField, Min(0f)] private float stopDistance = 0.15f;

    private Rigidbody2D rb;
    private EnemyHealth health;
    private static readonly int IsMovingHash =
    Animator.StringToHash("IsMoving");
    private void Awake()
    {
        CacheComponents();
    }

    private void CacheComponents()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (health == null)
            health = GetComponent<EnemyHealth>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>(true);
    }

    private void FixedUpdate()
    {
        if (!CanChase())
        {
            StopMovement();
            return;
        }

        Vector2 offset =
            (Vector2)target.transform.position - rb.position;

        float distance = offset.magnitude;

        if (distance <= stopDistance)
        {
            StopMovement();
            return;
        }

        // Không vượt quá khoảng cách còn lại trong một bước vật lý.
        float speed = Mathf.Min(
            moveSpeed,
            (distance - stopDistance) / Time.fixedDeltaTime
        );

        rb.linearVelocity = offset / distance * speed;

        if (animator != null)
            animator.SetBool(IsMovingHash, speed > 0f);
    }

    private bool CanChase()
    {
        return health.IsAlive
            && target != null
            && target.gameObject.activeInHierarchy
            && target.IsAlive;
    }

    private void StopMovement()
    {
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        // Chỉ có clip Run nên tạm dừng animation khi ngừng đuổi.
        if (animator != null)
            animator.SetBool(IsMovingHash, false);
    }
    public void PrepareSpawn(
    EnemyData data,
    PlayerHealth player,
    Vector3 position)
    {
        CacheComponents();

        target = player;
        moveSpeed = Mathf.Max(0f, data.moveSpeed);
        stopDistance = Mathf.Max(0f, data.stopDistance);

        health.ResetHealth(data.maxHealth);

        transform.position = position;

        // Reset trạng thái vật lý từ lần sử dụng trước.
        rb.position = new Vector2(position.x, position.y);
        rb.rotation = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
    private void OnEnable()
    {
        CacheComponents();

        if (animator != null &&
            animator.runtimeAnimatorController != null)
        {
            animator.speed = 1f;
            animator.Rebind();
            animator.Update(0f);
            animator.SetBool(IsMovingHash, false);
        }
    }
    private void OnDisable()
    {
        StopMovement();
    }

    // Dùng về sau khi spawner tạo enemy.
    public void SetTarget(PlayerHealth player)
    {
        target = player;
    }
}