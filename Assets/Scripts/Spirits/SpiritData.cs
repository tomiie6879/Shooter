using UnityEngine;

[CreateAssetMenu(menuName = "Shooter/Data/Spirit", fileName = "SD_Spirit")]
public sealed class SpiritData : ScriptableObject
{
    public string spiritId;
    public SpiritController prefab;
    public ProjectileData projectile;
    [Min(.1f)] public float range = 6;
    [Min(.05f)] public float cooldown = .7f;
    [Min(0)] public float damage = 20;
    public bool IsValid => !string.IsNullOrWhiteSpace(spiritId) && prefab != null &&
        prefab.HasAttack && projectile != null && Finite(range, .1f) &&
        Finite(cooldown, .05f) && Finite(damage, 0);
    private static bool Finite(float value, float min) =>
        !float.IsNaN(value) && !float.IsInfinity(value) && value >= min;
}
