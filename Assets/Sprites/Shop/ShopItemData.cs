using UnityEngine;

[CreateAssetMenu(
    fileName = "SI_Heal",
    menuName = "Shooter/Data/Shop Item"
)]
public sealed class ShopItemData : ScriptableObject
{
    public string displayName = "Hồi máu";

    [Min(1)]
    public int energyCost = 5;

    [Min(1f)]
    public float healAmount = 20f;
}