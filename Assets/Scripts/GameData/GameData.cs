using UnityEngine;

[CreateAssetMenu(menuName = "Shooter/Data/Game Data")]
public sealed class GameData : ScriptableObject
{
    [Header("Spirits")]
    public SpiritLoadout spiritLoadout;

    [Header("Loot")]
    public LootData goldLoot;
    public LootData energyLoot;

    public ProjectileData[] projectiles;
    public VfxData[] vfx;

    // Giữ lại danh sách cũ.
    // Pool Enemy mới sẽ đọc từ LevelData.
    public EnemyData[] enemies;

    [Header("Levels")]
    public string gameplaySceneName = "Battle_01";
    public LevelData[] levels;
}