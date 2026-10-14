using UnityEngine;

public readonly struct SpiritContext
{
    public readonly PlayerHealth Player;
    public readonly CombatPoolManager Pool;
    public readonly LayerMask EnemyLayer;
    public readonly SpiritTargetCoordinator Targets;
    public readonly float InitialAttackDelay;
    public readonly float TargetReviewInterval;
    public SpiritContext(PlayerHealth player, CombatPoolManager pool, LayerMask enemyLayer,
        SpiritTargetCoordinator targets, float initialAttackDelay, float targetReviewInterval)
    {
        Player = player; Pool = pool; EnemyLayer = enemyLayer; Targets = targets;
        InitialAttackDelay = initialAttackDelay;
        TargetReviewInterval = targetReviewInterval;
    }
}
public interface ISpiritAttack
{
    void Initialize(SpiritContext context, SpiritData data);
    void SetCombatEnabled(bool enabled);
}
