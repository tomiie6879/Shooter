using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-50)]
public sealed class WaveManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth player;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private CombatPoolManager poolManager;
    [SerializeField] private SpiritLightningAttack[] spiritAttacks;
    [SerializeField] private RunCurrency currency;
    [SerializeField] private ShopUI shopUI;

    [Header("UI")]
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Image waveFill;
    [SerializeField] private GameObject victoryPopup;
    [SerializeField] private GameObject defeatPopup;
    [SerializeField] private TMP_Text victoryGoldText;

    private LevelData currentLevel;
    private int completedWaves;
    private bool finished;

    public bool IsRunning { get; private set; }
    public bool IsShopping { get; private set; }
    public bool HasWon { get; private set; }
    public int CurrentWave { get; private set; }
    public float RemainingTime { get; private set; }

    private void Awake()
    {
        SetAttacksEnabled(false);

        if (victoryPopup != null)
            victoryPopup.SetActive(false);

        if (defeatPopup != null)
            defeatPopup.SetActive(false);
    }

    public bool BeginLevel(LevelData level, GameData gameData)
    {
        if (level == null ||
            !level.IsValid() ||
            gameData == null ||
            player == null ||
            enemySpawner == null ||
            !enemySpawner.isActiveAndEnabled ||
            poolManager == null ||
            !poolManager.IsInitialized ||
            currency == null ||
            shopUI == null ||
            victoryPopup == null)
        {
            Debug.LogError("WaveManager thiếu reference hoặc data.", this);
            return false;
        }

        currentLevel = level;
        completedWaves = 0;
        finished = false;
        HasWon = false;
        IsShopping = false;
        IsRunning = false;

        currency.BeginLevel();
        poolManager.ConfigureLoot(player, currency);

        if (!shopUI.Initialize(
                player,
                currency,
                this,
                gameData.shopItems))
        {
            return false;
        }

        BeginWave(1);
        return true;
    }

    private void BeginWave(int number)
    {
        CurrentWave = number;

        WaveSettings wave = currentLevel.waves[number - 1];
        RemainingTime = wave.duration;

        IsShopping = false;
        IsRunning = true;

        currency.CollectionEnabled = true;

        enemySpawner.BeginWave(wave);
        SetAttacksEnabled(true);

        Time.timeScale = 1f;
        RefreshUI();
    }

    private void Update()
    {
        if (!IsRunning)
            return;

        if (!player.IsAlive)
        {
            FinishBattle(false);
            return;
        }

        RemainingTime = Mathf.Max(
            0f,
            RemainingTime - Time.deltaTime
        );

        if (RemainingTime <= 0f)
        {
            completedWaves++;

            if (completedWaves >= currentLevel.WaveCount)
            {
                FinishBattle(true);
                return;
            }

            OpenShop();
            return;
        }

        RefreshUI();
    }

    private void OpenShop()
    {
        IsRunning = false;
        IsShopping = true;

        enemySpawner.StopSpawning();
        SetAttacksEnabled(false);
        currency.CollectionEnabled = false;

        // Giữ Enemy, Projectile và Loot tại chỗ.
        Time.timeScale = 0f;

        RefreshUI();
        shopUI.Open();
    }

    public void ContinueAfterShop()
    {
        // Ngăn nhấn nhiều lần làm nhảy wave.
        if (!IsShopping || finished)
            return;

        if (!player.IsAlive)
        {
            FinishBattle(false);
            return;
        }

        IsShopping = false;
        shopUI.Close();

        BeginWave(CurrentWave + 1);
    }

    private void FinishBattle(bool victory)
    {
        if (finished)
            return;

        finished = true;
        IsRunning = false;
        IsShopping = false;
        HasWon = victory;

        enemySpawner.StopSpawning();
        SetAttacksEnabled(false);

        currency.CollectionEnabled = false;
        shopUI.Close();

        int reward = currency.Settle(victory);

        if (victoryGoldText != null)
            victoryGoldText.text = reward.ToString();

        poolManager.ReturnAll();

        RefreshUI();

        victoryPopup.SetActive(victory);

        if (defeatPopup != null)
            defeatPopup.SetActive(!victory);

        Time.timeScale = 0f;
    }

    private void SetAttacksEnabled(bool value)
    {
        if (spiritAttacks == null)
            return;

        foreach (SpiritLightningAttack attack in spiritAttacks)
        {
            if (attack != null)
                attack.enabled = value;
        }
    }

    private void RefreshUI()
    {
        int total = currentLevel.WaveCount;

        if (waveText != null)
            waveText.text = $"WAVE {CurrentWave}/{total}";

        if (timerText != null)
        {
            timerText.text =
                Mathf.CeilToInt(RemainingTime).ToString("00");
        }

        if (waveFill != null)
            waveFill.fillAmount = (float)completedWaves / total;
    }
}