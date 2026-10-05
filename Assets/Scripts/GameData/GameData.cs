using UnityEngine;

[CreateAssetMenu(menuName = "Shooter/Data/Game Data")]
public sealed class GameData : ScriptableObject
{
    public ProjectileData[] projectiles;
    public VfxData[] vfx;
    public EnemyData[] enemies;
}