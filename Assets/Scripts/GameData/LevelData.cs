using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class SpawnSchedule
{
    public EnemyData enemy;
    [Min(0)] public float startTime;
    [Min(0)] public float endTime = 120;
    [Min(.1f)] public float interval = 2;
    [Min(1)] public int countPerSpawn = 1;
    [Min(1)] public int maxAlive = 20;
    [Tooltip("-1: rotate through all map points; otherwise use this point index.")]
    public int spawnPointIndex = -1;
}
[Serializable]
public sealed class BossSchedule
{
    public EnemyData enemy;
    [Min(0)] public float spawnTime = 105;
    public int spawnPointIndex = -1;
}
[CreateAssetMenu(fileName = "LD_Level", menuName = "Shooter/Data/Level Data")]
public sealed class LevelData : ScriptableObject
{
    public string levelId = "level_01";
    public string displayName = "Màn 1";
    public MapReferences mapPrefab;
    [Min(1)] public float duration = 120;
    public SpawnSchedule[] spawnSchedules = Array.Empty<SpawnSchedule>();
    [Tooltip("Leave Enemy empty for no boss. Assigned boss must be defeated.")]
    public BossSchedule boss = new BossSchedule();
    public bool HasBoss => boss != null && boss.enemy != null;
    public IEnumerable<EnemyData> Enemies()
    {
        foreach (var s in spawnSchedules) yield return s.enemy;
        if (HasBoss) yield return boss.enemy;
    }
    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(levelId) || mapPrefab == null ||
            !mapPrefab.IsValid() || !Number(duration, 1) || spawnSchedules == null) return false;
        foreach (var s in spawnSchedules)
            if (s == null || s.enemy == null || !Number(s.startTime, 0) ||
                !Number(s.endTime, 0) || s.endTime <= s.startTime || s.endTime > duration ||
                !Number(s.interval, .1f) || s.countPerSpawn < 1 || s.maxAlive < 1 ||
                !ValidPoint(s.spawnPointIndex)) return false;
        return !HasBoss || (Number(boss.spawnTime, 0) && boss.spawnTime <= duration && ValidPoint(boss.spawnPointIndex));
    }
    private bool ValidPoint(int index) => index >= -1 && index < mapPrefab.enemySpawnPoints.Length;
    private static bool Number(float v, float min) => !float.IsNaN(v) && !float.IsInfinity(v) && v >= min;
}
