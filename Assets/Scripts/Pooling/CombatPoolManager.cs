using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public sealed class CombatPoolManager : MonoBehaviour
{
    [SerializeField] private GameData gameData;

    private readonly Dictionary<ScriptableObject, Queue<PoolItem>>
        pools = new Dictionary<ScriptableObject, Queue<PoolItem>>();

    private readonly List<PoolItem> allItems =
        new List<PoolItem>();

    private Transform inactiveRoot;

    private void Awake()
    {
        if (gameData == null)
        {
            Debug.LogError("CombatPoolManager chưa được gán GameData.", this);
            enabled = false;
            return;
        }

        GameObject storage = new GameObject("Inactive");
        storage.transform.SetParent(transform, false);
        storage.SetActive(false);
        inactiveRoot = storage.transform;

        if (gameData.projectiles != null)
        {
            foreach (ProjectileData data in gameData.projectiles)
            {
                if (data == null)
                    continue;

                Prewarm<LightningProjectile>(
                    data, data.prefab, data.prewarmCount
                );

                // Tự đăng ký VFX được projectile tham chiếu.
                RegisterVfx(data.castVfx);
                RegisterVfx(data.hitVfx);
            }
        }

        if (gameData.vfx != null)
        {
            foreach (VfxData data in gameData.vfx)
                RegisterVfx(data);
        }
        if (gameData.enemies != null)
        {
            foreach (EnemyData data in gameData.enemies)
            {
                if (data == null)
                    continue;

                if (data.prefab == null ||
                    data.prefab.GetComponent<EnemyHealth>() == null ||
                    data.prefab.GetComponent<Rigidbody2D>() == null)
                {
                    Debug.LogError(
                        $"EnemyData {data.name}: prefab thiếu EnemyHealth hoặc Rigidbody2D.",
                        data
                    );

                    continue;
                }

                Prewarm<EnemyChase>(
                    data,
                    data.prefab,
                    data.prewarmCount
                );
            }
        }
    }

    private void RegisterVfx(VfxData data)
    {
        if (data == null)
            return;

        Prewarm<PooledVfx>(
            data, data.prefab, data.prewarmCount
        );
    }
    public bool TrySpawnEnemy(
    EnemyData data,
    Vector3 position,
    PlayerHealth player)
    {
        if (data == null ||
            player == null ||
            !player.gameObject.activeInHierarchy ||
            !player.IsAlive)
        {
            return false;
        }

        PoolItem item = Rent(
            data,
            position,
            Quaternion.identity,
            float.PositiveInfinity
        );

        if (item == null)
            return false;

        EnemyChase enemy = item.GetComponent<EnemyChase>();

        // Reset HP, target và vận tốc khi object còn đang tắt.
        enemy.PrepareSpawn(data, player, position);

        item.gameObject.SetActive(true);
        return true;
    }
    private void Prewarm<T>(
        ScriptableObject key,
        GameObject prefab,
        int count) where T : Component
    {
        if (pools.ContainsKey(key))
            return;

        if (prefab == null ||
            prefab.GetComponent<PoolItem>() == null ||
            prefab.GetComponent<T>() == null)
        {
            Debug.LogError(
                $"Data {key.name}: prefab thiếu PoolItem hoặc {typeof(T).Name}.",
                key
            );

            return;
        }

        count = Mathf.Max(1, count);
        Queue<PoolItem> queue = new Queue<PoolItem>(count);
        pools.Add(key, queue);

        for (int i = 0; i < count; i++)
        {
            // Tạo dưới cha đang tắt để chưa kích hoạt gameplay.
            GameObject instance = Instantiate(prefab, inactiveRoot);
            instance.name = $"{key.name}_{i:00}";
            instance.SetActive(false);

            PoolItem item = instance.GetComponent<PoolItem>();
            item.Initialize(this, key);

            queue.Enqueue(item);
            allItems.Add(item);
        }
    }

    private PoolItem Rent(
        ScriptableObject key,
        Vector3 position,
        Quaternion rotation,
        float lifetime)
    {
        if (key == null ||
            !pools.TryGetValue(key, out Queue<PoolItem> queue) ||
            queue.Count == 0)
        {
            return null;
        }

        PoolItem item = queue.Dequeue();

        // Vẫn tắt khi đặt vị trí và chuẩn bị dữ liệu.
        item.transform.SetParent(transform, false);
        item.transform.SetPositionAndRotation(position, rotation);
        item.BeginLease(lifetime);

        return item;
    }

    public bool TryFire(
        ProjectileData data,
        Vector3 position,
        Vector2 direction,
        float damage,
        LayerMask enemyLayer)
    {
        if (data == null || direction.sqrMagnitude < 0.0001f)
            return false;

        PoolItem item = Rent(
            data,
            position,
            Quaternion.identity,
            data.maxLifetime
        );

        if (item == null)
            return false;

        LightningProjectile projectile =
            item.GetComponent<LightningProjectile>();

        projectile.Launch(
            data, direction, damage, enemyLayer, this
        );

        PlayVfx(data.castVfx, position);
        return true;
    }

    public void PlayVfx(VfxData data, Vector3 position)
    {
        if (data == null)
            return;

        PoolItem item = Rent(
            data,
            position,
            Quaternion.identity,
            data.duration
        );

        if (item == null)
            return;

        item.GetComponent<PooledVfx>().Play();
    }

    public void Return(PoolItem item)
    {
        if (item == null || !item.IsRented ||
            !pools.TryGetValue(item.Key, out Queue<PoolItem> queue))
            return;

        item.EndLease();
        item.gameObject.SetActive(false);
        item.transform.SetParent(inactiveRoot, false);
        queue.Enqueue(item);
    }

    public void ReturnAll()
    {
        foreach (PoolItem item in allItems)
        {
            if (item != null && item.IsRented)
                Return(item);
        }
    }
}