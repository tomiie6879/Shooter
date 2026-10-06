using UnityEngine;
using UnityEngine.UI;

public sealed class LevelLoader : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private GameData gameData;

    [Header("Scene References")]
    [SerializeField] private Transform mapRoot;
    [SerializeField] private PlayerHealth player;
    [SerializeField] private CombatPoolManager poolManager;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private WaveManager waveManager;

    [Header("Optional UI")]
    [SerializeField] private Button nextLevelButton;

    private int currentIndex = -1;

    private void Awake()
    {
        // Chờ khởi tạo xong rồi mới chạy gameplay.
        Time.timeScale = 0f;
    }

    private void Start()
    {
        if (gameData == null ||
            gameData.levels == null ||
            gameData.levels.Length == 0 ||
            mapRoot == null ||
            player == null ||
            poolManager == null ||
            !poolManager.isActiveAndEnabled ||
            enemySpawner == null ||
            !enemySpawner.isActiveAndEnabled ||
            waveManager == null ||
            !waveManager.isActiveAndEnabled)
        {
            Fail("LevelLoader thiếu data hoặc reference.");
            return;
        }

        // Kiểm tra mã màn không bị trùng.
        var ids = new System.Collections.Generic.HashSet<string>();

        foreach (LevelData entry in gameData.levels)
        {
            if (entry == null ||
                string.IsNullOrWhiteSpace(entry.levelId) ||
                !ids.Add(entry.levelId))
            {
                Fail("Danh sách Levels có phần tử trống hoặc Level Id trùng.");
                return;
            }
        }

        string selectedId = GameSession.SelectedLevelId;

        // Play trực tiếp scene trong Editor thì thử màn đầu tiên.
        currentIndex = string.IsNullOrEmpty(selectedId) ? 0 : -1;

        if (!string.IsNullOrEmpty(selectedId))
        {
            for (int i = 0; i < gameData.levels.Length; i++)
            {
                if (gameData.levels[i].levelId == selectedId)
                {
                    currentIndex = i;
                    break;
                }
            }
        }

        if (currentIndex < 0)
        {
            Fail("Không tìm thấy Level Id đã chọn.");
            return;
        }

        LevelData level = gameData.levels[currentIndex];

        if (!level.IsValid())
        {
            Fail("LevelData chưa đủ Map, Spawn Point hoặc Wave.");
            return;
        }

        MapReferences map = Instantiate(
            level.mapPrefab,
            mapRoot
        );

        Vector3 position = map.playerSpawn.position;

        player.transform.position = position;

        if (player.TryGetComponent(out Rigidbody2D body))
        {
            body.position = (Vector2)position;
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
        }

        if (!poolManager.Initialize(gameData, level))
        {
            Fail("Không khởi tạo được pool. Kiểm tra lỗi phía trên.");
            return;
        }

        enemySpawner.Initialize(
            poolManager,
            player,
            map.enemySpawnPoints
        );

        if (nextLevelButton != null)
        {
            nextLevelButton.interactable =
                currentIndex + 1 < gameData.levels.Length;
        }

        if (!waveManager.BeginLevel(level, gameData))
        {
            Fail("Không bắt đầu được wave.");
            return;
        }

        Time.timeScale = 1f;
    }

    public void NextLevel()
    {
        if (!waveManager.HasWon)
            return;

        int nextIndex = currentIndex + 1;

        if (nextIndex >= gameData.levels.Length)
            return;

        GameSession.PlayLevel(gameData, nextIndex);
    }

    public void ReplayLevel()
    {
        if (currentIndex >= 0)
            GameSession.PlayLevel(gameData, currentIndex);
    }

    private void Fail(string message)
    {
        Debug.LogError(message, this);

        if (enemySpawner != null)
            enemySpawner.StopSpawning();

        Time.timeScale = 0f;
        enabled = false;
    }
}