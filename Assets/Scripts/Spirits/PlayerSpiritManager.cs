using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerHealth))]
public sealed class PlayerSpiritManager : MonoBehaviour
{
    [SerializeField] private Transform[] spiritPoints = new Transform[4];
    [SerializeField] private LayerMask enemyLayer;
    [Header("Target Sharing")]
    [SerializeField, Min(.1f)] private float targetReviewInterval = .75f;
    [SerializeField, Min(0)] private float initialShotSpacing = .15f;
    private readonly SpiritTargetCoordinator targets = new SpiritTargetCoordinator();
    [Header("Shared Orbit")]
    [Tooltip("Optional. If empty, use the average position of the configured SpiritPoints.")]
    [SerializeField] private Transform orbitCenter;
    [SerializeField] private Vector3 orbitCenterOffset;
    [SerializeField, Min(0)] private float orbitRadius = .45f;
    [SerializeField, Min(0)] private float radiusStep = .12f;
    [SerializeField, Min(0)] private float orbitSpeed = 45f;
    [SerializeField] private bool alternateDirections = true;
    [SerializeField] private float startingAngle;
    private readonly List<SpiritController> spirits = new List<SpiritController>();
    public bool IsInitialized { get; private set; }

    [ContextMenu("Collect SpiritPoints (sorted by name)")]
    private void CollectPoints()
    {
        var points = new List<Transform>();
        foreach (var child in GetComponentsInChildren<Transform>(true))
            if (child.name.StartsWith("SpiritPoint_", System.StringComparison.Ordinal))
                points.Add(child);
        points.Sort((a,b) => string.CompareOrdinal(a.name, b.name));
        spiritPoints = points.ToArray();
    }
    public bool Initialize(SpiritLoadout loadout, CombatPoolManager pool)
    {
        if (IsInitialized) return true;
        if (loadout == null || loadout.slots == null || spiritPoints == null ||
            loadout.slots.Length > spiritPoints.Length || pool == null || !pool.IsInitialized ||
            enemyLayer.value == 0) return Fail("Thiếu Loadout, SpiritPoints, pool hoặc Enemy Layer.");
        var used = new HashSet<Transform>();
        for (int i = 0; i < loadout.slots.Length; i++)
        {
            var data = loadout.slots[i];
            if (data == null) continue;
            var point = spiritPoints[i];
            if (!data.IsValid || point == null || !point.IsChildOf(transform) ||
                !point.gameObject.activeInHierarchy || !used.Add(point))
                return Fail($"Spirit slot {i + 1}: data/prefab/point không hợp lệ hoặc point bị trùng.");
        }
        // Prepare all resources before creating any instance.
        foreach (var data in loadout.slots)
            if (data != null && !pool.EnsureProjectilePool(data.projectile)) return false;
        int count = 0;
        foreach (var data in loadout.slots) if (data != null) count++;
        Transform center = orbitCenter != null ? orbitCenter : transform;
        Vector3 offset = orbitCenterOffset;
        if (orbitCenter == null)
        {
            Vector3 sum = Vector3.zero;
            int pointCount = 0;
            foreach (var point in spiritPoints)
                if (point != null)
                {
                    sum += transform.InverseTransformPoint(point.position);
                    pointCount++;
                }
            if (pointCount > 0) offset += sum / pointCount;
        }
        int orbitIndex = 0;
        for (int i = 0; i < loadout.slots.Length; i++)
        {
            var data = loadout.slots[i];
            if (data == null) continue;
            var spirit = Instantiate(data.prefab, spiritPoints[i], false);
            spirit.transform.localPosition = Vector3.zero;
            spirit.transform.localRotation = Quaternion.identity;
            // Disable prefab-local orbits so only the root controls movement.
            foreach (var oldOrbit in spirit.GetComponentsInChildren<SpiritOrbit>(true))
                oldOrbit.enabled = false;
            var orbit = spirit.GetComponent<SpiritOrbit>();
            if (orbit == null) orbit = spirit.gameObject.AddComponent<SpiritOrbit>();
            bool reverse = alternateDirections && orbitIndex % 2 == 1;
            orbit.Configure(center, offset, orbitRadius + (reverse ? radiusStep : 0),
                orbitSpeed * (reverse ? -1 : 1), startingAngle + 360f * orbitIndex / count);
            var context = new SpiritContext(GetComponent<PlayerHealth>(), pool, enemyLayer,
                targets, orbitIndex * Mathf.Max(0, initialShotSpacing), Mathf.Max(.1f, targetReviewInterval));
            orbitIndex++;
            spirit.Initialize(context, data);
            spirit.gameObject.SetActive(true);
            spirits.Add(spirit);
        }
        IsInitialized = true;
        return true;
    }
    public void SetCombatEnabled(bool value)
    {
        foreach (var spirit in spirits)
            if (spirit != null) spirit.SetCombatEnabled(value);
        if (!value) targets.Clear();
    }
    private bool Fail(string message)
    { Debug.LogError("PlayerSpiritManager: " + message, this); return false; }
    private void OnDisable() => SetCombatEnabled(false);
}
