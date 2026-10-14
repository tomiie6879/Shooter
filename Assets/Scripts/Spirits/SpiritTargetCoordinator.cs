using System.Collections.Generic;
using UnityEngine;

// One instance per player's team. Reservations refer to enemies, never individual colliders.
public sealed class SpiritTargetCoordinator
{
    private readonly Dictionary<object, EnemyHealth> assignments = new Dictionary<object, EnemyHealth>();
    private readonly HashSet<EnemyHealth> watched = new HashSet<EnemyHealth>();
    private readonly List<object> expired = new List<object>();

    public int CountOthers(object owner, EnemyHealth enemy)
    {
        int count = 0;
        foreach (var pair in assignments)
            if (!ReferenceEquals(pair.Key, owner) && pair.Value == enemy) count++;
        return count;
    }
    public bool IsAssigned(object owner, EnemyHealth enemy) =>
        assignments.TryGetValue(owner, out var assigned) && assigned == enemy;
    public void Assign(object owner, EnemyHealth enemy)
    {
        if (IsAssigned(owner, enemy)) return;
        Release(owner);
        if (enemy == null || !enemy.IsAlive || !enemy.gameObject.activeInHierarchy) return;
        assignments[owner] = enemy;
        if (watched.Add(enemy))
        {
            enemy.Died += Invalidate;
            enemy.Despawned += Invalidate;
        }
    }
    public void Release(object owner)
    {
        if (!assignments.TryGetValue(owner, out var enemy)) return;
        assignments.Remove(owner);
        foreach (var pair in assignments) if (pair.Value == enemy) return;
        Unwatch(enemy);
    }
    private void Invalidate(EnemyHealth enemy)
    {
        expired.Clear();
        foreach (var pair in assignments) if (pair.Value == enemy) expired.Add(pair.Key);
        foreach (var owner in expired) assignments.Remove(owner);
        Unwatch(enemy);
        expired.Clear();
    }
    private void Unwatch(EnemyHealth enemy)
    {
        watched.Remove(enemy);
        if (enemy == null) return;
        enemy.Died -= Invalidate;
        enemy.Despawned -= Invalidate;
    }
    public void Clear()
    {
        foreach (var enemy in watched)
        {
            if (enemy == null) continue;
            enemy.Died -= Invalidate;
            enemy.Despawned -= Invalidate;
        }
        watched.Clear();
        assignments.Clear();
        expired.Clear();
    }
}
