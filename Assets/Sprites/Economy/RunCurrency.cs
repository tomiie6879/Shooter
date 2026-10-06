using System;
using TMPro;
using UnityEngine;

public sealed class RunCurrency : MonoBehaviour
{
    private const string WalletKey = "Shooter.Wallet.Gold";

    [Header("HUD")]
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text energyText;

    [Header("Optional persistent wallet text")]
    [SerializeField] private TMP_Text walletText;

    public int Gold { get; private set; }
    public int Energy { get; private set; }

    public int WalletGold =>
        PlayerPrefs.GetInt(WalletKey, 0);

    public bool CollectionEnabled { get; set; }

    private bool settled;

    public event Action Changed;

    public void BeginLevel()
    {
        Gold = 0;
        Energy = 0;
        settled = false;
        CollectionEnabled = false;
        Notify();
    }

    public void Collect(LootKind kind, int amount)
    {
        if (settled || !CollectionEnabled || amount <= 0)
            return;

        if (kind == LootKind.Gold)
            Gold += amount;
        else
            Energy += amount;

        Notify();
    }

    public bool TrySpendEnergy(int amount)
    {
        if (settled || amount < 0 || Energy < amount)
            return false;

        Energy -= amount;
        Notify();
        return true;
    }

    // Trả về số Gold được thưởng.
    public int Settle(bool victory)
    {
        if (settled)
            return 0;

        settled = true;
        CollectionEnabled = false;

        int reward = victory ? Gold : 0;

        if (victory)
        {
            PlayerPrefs.SetInt(WalletKey, WalletGold + reward);
            PlayerPrefs.Save();
        }
        else
        {
            Gold = 0;
        }

        Notify();
        return reward;
    }

    private void Notify()
    {
        if (goldText != null)
            goldText.text = Gold.ToString();

        if (energyText != null)
            energyText.text = Energy.ToString();

        if (walletText != null)
            walletText.text = WalletGold.ToString();

        Changed?.Invoke();
    }
}