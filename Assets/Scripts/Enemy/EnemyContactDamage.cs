using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(EnemyHealth))]
public sealed class EnemyContactDamage : MonoBehaviour
{
    [SerializeField, Min(0f)] private float damage = 10f;
    [SerializeField, Min(0.01f)] private float damageInterval = 0.6f;

    private EnemyHealth health;
    private float nextDamageTime;

    private void Awake()
    {
        health = GetComponent<EnemyHealth>();
    }

    private void OnEnable()
    {
        // Each spawn gets its own fresh contact cooldown.
        nextDamageTime = Time.time;
    }

    public void Configure(float contactDamage)
    {
        damage = Mathf.Max(0f, contactDamage);
    }

    private void OnTriggerEnter2D(Collider2D other) => TryDamage(other);
    private void OnTriggerStay2D(Collider2D other) => TryDamage(other);
    private void OnCollisionEnter2D(Collision2D collision) => TryDamage(collision.collider);
    private void OnCollisionStay2D(Collision2D collision) => TryDamage(collision.collider);

    private void TryDamage(Collider2D other)
    {
        if (!isActiveAndEnabled || health == null || !health.IsAlive ||
            damage <= 0f || Time.time < nextDamageTime)
            return;

        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
        if (player == null || !player.isActiveAndEnabled || !player.IsAlive)
            return;

        // Set before damage: multiple colliders must not cause duplicate hits.
        nextDamageTime = Time.time + Mathf.Max(0.01f, damageInterval);
        player.TakeDamage(damage);
    }
}
