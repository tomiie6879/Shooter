using System.Collections.Generic;
using TMPro;
using UnityEngine;

public sealed class ShopUI : MonoBehaviour
{
    [SerializeField] private GameObject popup;
    [SerializeField] private Transform content;
    [SerializeField] private ShopItemButton buttonPrefab;
    [SerializeField] private TMP_Text energyText;

    private PlayerHealth player;
    private RunCurrency currency;
    private WaveManager waveManager;

    private readonly List<ShopItemButton> buttons =
        new List<ShopItemButton>();

    private bool initialized;

    public bool Initialize(
        PlayerHealth target,
        RunCurrency runCurrency,
        WaveManager manager,
        ShopItemData[] items)
    {
        if (initialized)
            return true;

        if (popup == null ||
            content == null ||
            buttonPrefab == null ||
            target == null ||
            runCurrency == null ||
            manager == null)
        {
            Debug.LogError("ShopUI chưa được gán đủ thông tin.", this);
            return false;
        }

        player = target;
        currency = runCurrency;
        waveManager = manager;

        popup.SetActive(false);

        if (items != null)
        {
            foreach (ShopItemData item in items)
            {
                if (item == null)
                    continue;

                ShopItemButton row = Instantiate(buttonPrefab, content);
                row.gameObject.SetActive(true);
                row.Bind(item, this);
                buttons.Add(row);
            }
        }

        currency.Changed += Refresh;
        player.HealthChanged += OnHealthChanged;

        initialized = true;
        Refresh();
        return true;
    }

    public void Open()
    {
        popup.SetActive(true);
        Refresh();
    }

    public void Close()
    {
        popup.SetActive(false);
    }

    public bool CanBuy(ShopItemData item)
    {
        return item != null &&
               waveManager != null &&
               waveManager.IsShopping &&
               player != null &&
               player.IsAlive &&
               player.CurrentHealth < player.MaxHealth &&
               item.energyCost > 0 &&
               item.healAmount > 0f &&
               currency.Energy >= item.energyCost;
    }

    public void Buy(ShopItemData item)
    {
        if (!CanBuy(item))
            return;

        if (!currency.TrySpendEnergy(item.energyCost))
            return;

        player.Heal(item.healAmount);
        Refresh();
    }

    private void OnHealthChanged(float current, float max)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (energyText != null && currency != null)
            energyText.text = currency.Energy.ToString();

        foreach (ShopItemButton button in buttons)
            button.Refresh();
    }

    private void OnDestroy()
    {
        if (currency != null)
            currency.Changed -= Refresh;

        if (player != null)
            player.HealthChanged -= OnHealthChanged;
    }
}