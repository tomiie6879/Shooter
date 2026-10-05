using UnityEngine;

[CreateAssetMenu(menuName = "Shooter/Data/Projectile")]
public sealed class ProjectileData : ScriptableObject
{
    public GameObject prefab;

    [Header("Pool")]
    [Min(1)] public int prewarmCount = 20;

    [Header("Movement")]
    [Min(0.1f)] public float speed = 12f;
    [Min(0.1f)] public float maxDistance = 8f;
    [Min(0.1f)] public float maxLifetime = 2f;

    [Header("Collision")]
    [Min(0.01f)] public float hitRadius = 0.1f;

    [Header("Effects")]
    public VfxData castVfx;
    public VfxData hitVfx;
}