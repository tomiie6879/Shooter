using UnityEngine;

[CreateAssetMenu(menuName = "Shooter/Data/Spirit Loadout", fileName = "SL_Player")]
public sealed class SpiritLoadout : ScriptableObject
{
    [Tooltip("Index 0 uses SpiritPoint 01. Empty entries leave the corresponding slot empty.")]
    public SpiritData[] slots = new SpiritData[4];
}
