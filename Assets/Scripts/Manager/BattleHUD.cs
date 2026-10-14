using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Presentation only; no spawning, rewards or battle state transitions.
public sealed class BattleHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private Image waveFill;
    [SerializeField] private GameObject victoryPopup;
    [SerializeField] private GameObject defeatPopup;
    [SerializeField] private TMP_Text victoryGoldText;
    public bool IsConfigured => victoryPopup != null;

    public void ResetView()
    {
        if (victoryPopup != null) victoryPopup.SetActive(false);
        if (defeatPopup != null) defeatPopup.SetActive(false);
    }
    public void Refresh(BattleProgress progress)
    {
        if (waveText != null) waveText.text = "BATTLE";
        if (waveFill != null) waveFill.fillAmount = progress.Elapsed / progress.Duration;
    }
    public void ShowResult(bool victory, int gold)
    {
        if (victoryGoldText != null) victoryGoldText.text = gold.ToString();
        if (victoryPopup != null) victoryPopup.SetActive(victory);
        if (defeatPopup != null) defeatPopup.SetActive(!victory);
    }

}
