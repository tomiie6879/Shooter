using UnityEngine;

[DefaultExecutionOrder(-50)]
public sealed class BattleManager : MonoBehaviour
{
    [SerializeField] private PlayerHealth player;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private CombatPoolManager poolManager;
    private PlayerSpiritManager spiritManager;
    [SerializeField] private RunCurrency currency;
    [SerializeField] private BattleHUD hud;
    public bool IsRunning { get; private set; }
    public bool HasWon { get; private set; }
    public float RemainingTime => progress == null ? 0 : progress.Remaining;
    private BattleProgress progress;
    private bool finished;

    private void Awake()
    {
        SetAttacks(false);
        if (hud != null) hud.ResetView();
    }
    public bool BeginLevel(LevelData level)
    {
        if (level == null || !level.IsValid() || player == null ||
            enemySpawner == null || !enemySpawner.isActiveAndEnabled ||
            poolManager == null || !poolManager.IsInitialized || currency == null || hud == null || !hud.IsConfigured)
        {
            Debug.LogError("BattleManager thiếu reference hoặc LevelData không hợp lệ.", this);
            return false;
        }
        spiritManager = player.GetComponent<PlayerSpiritManager>();
        if (spiritManager == null || !spiritManager.IsInitialized)
        {
            Debug.LogError("PlayerSpiritManager chưa được khởi tạo.", this);
            return false;
        }
        Unsubscribe();
        progress = new BattleProgress(level.duration, level.HasBoss);
        finished = HasWon = false;
        hud.ResetView();
        currency.BeginLevel();
        poolManager.ConfigureLoot(player, currency);
        enemySpawner.BossSpawned += OnBossSpawned;
        enemySpawner.BossDefeated += OnBossDefeated;
        enemySpawner.BeginBattle(level);
        currency.CollectionEnabled = true;
        IsRunning = true;
        SetAttacks(true);
        enemySpawner.Tick(0);
        RefreshUI();
        return true;
    }
    private void Update()
    {
        if (!IsRunning) return;
        if (!player.IsAlive) { Finish(false); return; }
        if (Time.timeScale <= 0) return;
        progress.Tick(Time.deltaTime);
        enemySpawner.Tick(progress.Elapsed);
        if (progress.CanWin) { Finish(true); return; }
        RefreshUI();
    }
    private void OnBossSpawned() => progress.MarkBossSpawned();
    private void OnBossDefeated() => progress.MarkBossDefeated();
    private void Finish(bool victory)
    {
        if (finished) return;
        finished = true;
        IsRunning = false;
        HasWon = victory;
        enemySpawner.StopSpawning();
        Unsubscribe();
        SetAttacks(false);
        currency.CollectionEnabled = false;
        int reward = currency.Settle(victory);
        poolManager.ReturnAll();
        RefreshUI();
        hud.ShowResult(victory, reward);
        Time.timeScale = 0;
    }
    private void SetAttacks(bool value)
    {
        if (spiritManager != null) spiritManager.SetCombatEnabled(value);
    }
    private void RefreshUI() => hud.Refresh(progress);
    private void Unsubscribe()
    {
        if (enemySpawner == null) return;
        enemySpawner.BossSpawned -= OnBossSpawned;
        enemySpawner.BossDefeated -= OnBossDefeated;
    }
    private void OnDisable()
    {
        Unsubscribe();
        if (enemySpawner != null) enemySpawner.StopSpawning();
        SetAttacks(false);
        if (currency != null) currency.CollectionEnabled = false;
        IsRunning = false;
    }
}
