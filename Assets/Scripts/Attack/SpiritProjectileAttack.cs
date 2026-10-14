using System.Collections.Generic;
using UnityEngine;

public sealed class SpiritProjectileAttack : MonoBehaviour, ISpiritAttack
{
    [SerializeField] private Transform firePoint;
    private SpiritContext context;
    private SpiritData data;
    private bool combatEnabled;
    private float nextAttackTime, nextTargetReview;
    private ContactFilter2D filter;
    private Collider2D targetCollider;
    private EnemyHealth targetHealth;
    private readonly List<Collider2D> candidates = new List<Collider2D>(32);

    public void Initialize(SpiritContext context, SpiritData data)
    {
        ReleaseTarget();
        this.context = context;
        this.data = data;
        if (firePoint == null) firePoint = transform;
        filter = new ContactFilter2D();
        filter.SetLayerMask(context.EnemyLayer);
        filter.useTriggers = true;
        combatEnabled = false;
    }
    public void SetCombatEnabled(bool value)
    {
        if (combatEnabled == value) return;
        combatEnabled = value;
        ReleaseTarget();
        if (value)
        {
            nextAttackTime = Time.time + context.InitialAttackDelay;
            nextTargetReview = Time.time;
        }
    }
    private bool TargetValid(Vector2 origin)
    {
        return targetHealth != null && targetHealth.IsAlive &&
            targetHealth.gameObject.activeInHierarchy &&
            targetCollider != null && targetCollider.enabled &&
            targetCollider.gameObject.activeInHierarchy &&
            (context.EnemyLayer.value & (1 << targetCollider.gameObject.layer)) != 0 &&
            ((Vector2)targetCollider.ClosestPoint(origin) - origin).sqrMagnitude <= data.range * data.range &&
            context.Targets.IsAssigned(this, targetHealth);
    }
    private void LateUpdate()
    {
        if (!combatEnabled || data == null || context.Player == null || !context.Player.IsAlive)
        { ReleaseTarget(); return; }
        if (Time.timeScale <= 0) return;
        Vector2 origin = firePoint.position;
        bool hadTarget = targetCollider != null;
        bool valid = TargetValid(origin);
        // Invalid leases (death/despawn/out of range) are released immediately, even during cooldown.
        if (!valid) ReleaseTarget();
        if (Time.time >= nextTargetReview || (!valid && hadTarget))
            SelectTarget(origin);
        if (targetHealth == null || Time.time < nextAttackTime) return;
        Vector2 direction = (Vector2)targetCollider.bounds.center - origin;
        if (direction.sqrMagnitude < .0001f) direction = Vector2.right;
        bool fired = context.Pool.TryFire(data.projectile, firePoint.position,
            direction, data.damage, context.EnemyLayer);
        nextAttackTime = Time.time + (fired ? data.cooldown : .1f);
    }
    private void SelectTarget(Vector2 origin)
    {
        Physics2D.OverlapCircle(origin, data.range, filter, candidates);
        Collider2D best = null;
        EnemyHealth bestHealth = null;
        int least = int.MaxValue;
        float nearest = float.PositiveInfinity;
        foreach (var candidate in candidates)
        {
            var health = candidate.GetComponentInParent<EnemyHealth>();
            if (health == null || !health.IsAlive || !health.gameObject.activeInHierarchy) continue;
            int count = context.Targets.CountOthers(this, health);
            float distance = ((Vector2)candidate.bounds.center - origin).sqrMagnitude;
            if (count > least || (count == least && distance >= nearest)) continue;
            least = count; nearest = distance; best = candidate; bestHealth = health;
        }
        context.Targets.Assign(this, bestHealth);
        targetCollider = best;
        targetHealth = bestHealth;
        nextTargetReview = Time.time + (best == null ? .1f : context.TargetReviewInterval);
    }
    private void ReleaseTarget()
    {
        context.Targets?.Release(this);
        targetCollider = null;
        targetHealth = null;
    }
    private void OnDisable()
    {
        combatEnabled = false;
        ReleaseTarget();
    }
}
