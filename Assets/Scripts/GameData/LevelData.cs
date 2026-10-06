using System;
using UnityEngine;

[Serializable]
public sealed class EnemySpawnGroup
{
    public EnemyData enemy;

    [Min(0.1f)]
    public float interval = 2f;
}

[Serializable]
public sealed class WaveSettings
{
    [Min(1f)]
    public float duration = 60f;

    public EnemySpawnGroup[] spawnGroups =
        new EnemySpawnGroup[0];
}

[CreateAssetMenu(
    fileName = "LD_Level",
    menuName = "Shooter/Data/Level Data"
)]
public sealed class LevelData : ScriptableObject
{
    [Tooltip("Mã riêng của màn, ví dụ level_01.")]
    public string levelId = "level_01";

    public string displayName = "Màn 1";

    public MapReferences mapPrefab;

    public WaveSettings[] waves =
    {
        new WaveSettings()
    };

    public int WaveCount => waves == null ? 0 : waves.Length;

    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(levelId) ||
            mapPrefab == null ||
            !mapPrefab.IsValid() ||
            WaveCount == 0)
        {
            return false;
        }

        foreach (WaveSettings wave in waves)
        {
            if (wave == null ||
                !ValidNumber(wave.duration, 1f) ||
                wave.spawnGroups == null ||
                wave.spawnGroups.Length == 0)
            {
                return false;
            }

            foreach (EnemySpawnGroup group in wave.spawnGroups)
            {
                if (group == null ||
                    group.enemy == null ||
                    !ValidNumber(group.interval, 0.1f))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static bool ValidNumber(float value, float minimum)
    {
        return !float.IsNaN(value) &&
               !float.IsInfinity(value) &&
               value >= minimum;
    }
}