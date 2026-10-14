using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class EnemySpawner : MonoBehaviour
{
    public event Action BossSpawned;
    public event Action BossDefeated;
    private CombatPoolManager pool;
    private PlayerHealth player;
    private Transform[] points;
    private LevelData level;
    private float[] next;
    private List<EnemyHealth>[] alive;
    private EnemyHealth boss;
    private bool bossSpawned, bossDefeated, running;
    private int pointIndex;
    public void Initialize(CombatPoolManager pool, PlayerHealth player, Transform[] points)
    {
        StopSpawning();
        this.pool = pool; this.player = player; this.points = points;
        pointIndex = 0;
    }
    public void BeginBattle(LevelData data)
    {
        StopSpawning();
        level = data;
        next = new float[data.spawnSchedules.Length];
        alive = new List<EnemyHealth>[next.Length];
        for (int i = 0; i < next.Length; i++)
        {
            next[i] = data.spawnSchedules[i].startTime;
            alive[i] = new List<EnemyHealth>();
        }
        bossSpawned = bossDefeated = false;
        running = true;
    }
    // A single clock owned by BattleManager drives all schedules.
    public void Tick(float elapsed)
    {
        if (!running || !player.IsAlive || !pool.IsInitialized) return;
        if (level.HasBoss && !bossSpawned && elapsed >= level.boss.spawnTime)
        {
            if (Spawn(level.boss.enemy, level.boss.spawnPointIndex, out boss))
            {
                bossSpawned = true;
                boss.Died += OnBossDied;
                BossSpawned?.Invoke();
            }
        }
        for (int i = 0; i < next.Length; i++)
        {
            var s = level.spawnSchedules[i];
            if (elapsed < next[i] || elapsed >= s.endTime || elapsed >= level.duration) continue;
            int count = Mathf.Min(s.countPerSpawn, s.maxAlive - alive[i].Count);
            for (int n = 0; n < count; n++)
            {
                if (level.HasBoss && !bossSpawned && s.enemy == level.boss.enemy &&
                    pool.AvailableEnemies(s.enemy) <= 1) break;
                if (!Spawn(s.enemy, s.spawnPointIndex, out var enemy)) break;
                alive[i].Add(enemy);
                enemy.Despawned += OnDespawned;
            }
            // Avoid catch-up bursts after a slow frame or a full pool.
            next[i] = elapsed + s.interval;
        }
    }
    private bool Spawn(EnemyData data, int index, out EnemyHealth enemy)
    {
        enemy = null;
        if (points == null || points.Length == 0) return false;
        if (index < 0) { index = pointIndex; pointIndex = (pointIndex + 1) % points.Length; }
        return index < points.Length && points[index] != null &&
            pool.TrySpawnEnemy(data, points[index].position, player, out enemy);
    }
    private void OnDespawned(EnemyHealth enemy)
    {
        enemy.Despawned -= OnDespawned;
        foreach (var list in alive) list.Remove(enemy);
    }
    private void OnBossDied(EnemyHealth enemy)
    {
        if (bossDefeated) return;
        bossDefeated = true;
        enemy.Died -= OnBossDied;
        BossDefeated?.Invoke();
    }
    public void StopSpawning()
    {
        running = false;
        if (boss != null) boss.Died -= OnBossDied;
        boss = null;
        if (alive != null)
            foreach (var list in alive)
                foreach (var enemy in list)
                    if (enemy != null) enemy.Despawned -= OnDespawned;
    }
    private void OnDisable() => StopSpawning();
}
