using UnityEngine;

public sealed class EnemySpawner : MonoBehaviour
{
    [SerializeField] private CombatPoolManager poolManager;
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private PlayerHealth player;

    [SerializeField] private Transform[] spawnPoints;
    [SerializeField, Min(0.1f)] private float interval = 2f;

    private float nextSpawnTime;
    private int pointIndex;

    private void Start()
    {
        if (poolManager == null ||
            enemyData == null ||
            player == null ||
            spawnPoints == null ||
            spawnPoints.Length == 0)
        {
            Debug.LogError("EnemySpawner chưa được gán đủ thông tin.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        if (!player.IsAlive ||
            !player.gameObject.activeInHierarchy ||
            Time.time < nextSpawnTime)
        {
            return;
        }

        Transform point = spawnPoints[pointIndex];
        pointIndex = (pointIndex + 1) % spawnPoints.Length;

        bool spawned = point != null &&
            poolManager.TrySpawnEnemy(
                enemyData,
                point.position,
                player
            );

        // Pool đầy thì đợi rồi thử lại, không Instantiate thêm.
        nextSpawnTime = Time.time + (spawned ? interval : 0.5f);
    }
}