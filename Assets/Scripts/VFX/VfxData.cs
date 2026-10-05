using UnityEngine;

[CreateAssetMenu(menuName = "Shooter/Data/VFX")]
public sealed class VfxData : ScriptableObject
{
    public GameObject prefab;

    [Min(1)] public int prewarmCount = 10;
    [Min(0.01f)] public float duration = 0.3f;
}