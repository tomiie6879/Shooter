using System;

// Scene-independent battle rules, driven by the controller's clock.
public sealed class BattleProgress
{
    public float Duration { get; }
    public float Elapsed { get; private set; }
    public float Remaining => Math.Max(0, Duration - Elapsed);
    public bool RequiresBoss { get; }
    public bool BossSpawned { get; private set; }
    public bool BossDefeated { get; private set; }
    public bool CanWin => Remaining <= 0 && (!RequiresBoss || BossDefeated);
    public BattleProgress(float duration, bool requiresBoss)
    {
        if (float.IsNaN(duration) || float.IsInfinity(duration) || duration <= 0)
            throw new ArgumentOutOfRangeException(nameof(duration));
        Duration = duration;
        RequiresBoss = requiresBoss;
    }
    public void Tick(float delta)
    {
        if (delta > 0 && !float.IsNaN(delta)) Elapsed = Math.Min(Duration, Elapsed + delta);
    }
    public void MarkBossSpawned() { if (RequiresBoss) BossSpawned = true; }
    public void MarkBossDefeated() { if (BossSpawned) BossDefeated = true; }
}
