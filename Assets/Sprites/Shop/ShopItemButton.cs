using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class ShopItemButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;

    private ShopItemData data;
    private ShopUI shop;

    public void Bind(ShopItemData item, ShopUI owner)
    {
        data = item;
        shop = owner;

        button.onClick.AddListener(Buy);

        Refresh();
    }

    private void Buy()
    {
        shop.Buy(data);
    }

    public void Refresh()
    {
        label.text =
            $"{data.displayName}\n" +
            $"+{data.healAmount:0} HP — {data.energyCost} Energy";

        button.interactable = shop.CanBuy(data);
    }
}