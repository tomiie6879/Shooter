using UnityEngine;

public enum LootKind
{
    Gold,
    EnergyShard
}

[CreateAssetMenu(
    fileName = "LD_Loot",
    menuName = "Shooter/Data/Loot"
)]
public sealed class LootData : ScriptableObject
{
    public LootKind kind;
    public GameObject prefab;

    [Min(1)] public int prewarmCount = 100;

    [Header("Attraction")]
    [Min(0.1f)] public float attractRadius = 2f;
    [Min(0.01f)] public float collectDistance = 0.15f;
    [Min(0f)] public float pickupDelay = 0.2f;
    [Min(0.1f)] public float initialSpeed = 3f;
    [Min(0f)] public float acceleration = 25f;
    [Min(0.1f)] public float maxSpeed = 18f;
}