using UnityEngine;

[CreateAssetMenu(menuName = "Shooter/Data/Enemy")]
public sealed class EnemyData : ScriptableObject
{
    [Header("Prefab")]
    public GameObject prefab;

    [Header("Pool")]
    [Min(1)] public int prewarmCount = 20;

    [Header("Stats")]
    [Min(1f)] public float maxHealth = 30f;
    [Min(0f)] public float moveSpeed = 2f;
    [Min(0f)] public float stopDistance = 0.15f;
}