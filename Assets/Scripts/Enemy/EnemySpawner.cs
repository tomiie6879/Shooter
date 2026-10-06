using UnityEngine;

public sealed class EnemySpawner : MonoBehaviour
{
    private CombatPoolManager poolManager;
    private PlayerHealth player;
    private Transform[] spawnPoints;

    private EnemySpawnGroup[] groups;
    private float[] nextSpawnTimes;

    private int pointIndex;
    private bool isSpawning;

    public void Initialize(
        CombatPoolManager pool,
        PlayerHealth target,
        Transform[] points)
    {
        StopSpawning();

        poolManager = pool;
        player = target;
        spawnPoints = points;
        pointIndex = 0;
    }

    public void BeginWave(WaveSettings wave)
    {
        StopSpawning();

        groups = wave.spawnGroups;
        nextSpawnTimes = new float[groups.Length];

        for (int i = 0; i < nextSpawnTimes.Length; i++)
        {
            // Mỗi nhóm có thể spawn ngay khi wave bắt đầu.
            nextSpawnTimes[i] = Time.time;
        }

        isSpawning = true;
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    private void Update()
    {
        if (!isSpawning ||
            poolManager == null ||
            !poolManager.IsInitialized ||
            player == null ||
            !player.IsAlive ||
            !player.gameObject.activeInHierarchy ||
            Time.timeScale <= 0f ||
            spawnPoints == null ||
            spawnPoints.Length == 0)
        {
            return;
        }

        for (int i = 0; i < groups.Length; i++)
        {
            if (Time.time < nextSpawnTimes[i])
                continue;

            Transform point = spawnPoints[pointIndex];
            pointIndex = (pointIndex + 1) % spawnPoints.Length;

            EnemySpawnGroup group = groups[i];

            bool spawned = point != null &&
                poolManager.TrySpawnEnemy(
                    group.enemy,
                    point.position,
                    player
                );

            nextSpawnTimes[i] =
                Time.time + (spawned ? group.interval : 0.5f);
        }
    }

    private void OnDisable()
    {
        StopSpawning();
    }
}