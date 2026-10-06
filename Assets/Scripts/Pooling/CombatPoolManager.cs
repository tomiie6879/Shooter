using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public sealed class CombatPoolManager : MonoBehaviour
{
    

    private readonly Dictionary<ScriptableObject, Queue<PoolItem>>
        pools = new Dictionary<ScriptableObject, Queue<PoolItem>>();

    private readonly List<PoolItem> allItems =
        new List<PoolItem>();

    private Transform inactiveRoot;
    private LootData goldLoot;
    private LootData energyLoot;
    private PlayerHealth lootPlayer;
    private RunCurrency runCurrency;

    public bool IsInitialized { get; private set; }

    public bool Initialize(GameData gameData, LevelData level)
    {
        if (IsInitialized)
            return true;

        if (gameData == null || level == null || !level.IsValid())
        {
            Debug.LogError("Data khởi tạo pool không hợp lệ.", this);
            return false;
        }

        if (inactiveRoot == null)
        {
            GameObject storage = new GameObject("Inactive");
            storage.transform.SetParent(transform, false);
            storage.SetActive(false);
            inactiveRoot = storage.transform;
        }

        // Projectile và VFX dùng chung.
        if (gameData.projectiles != null)
        {
            foreach (ProjectileData data in gameData.projectiles)
            {
                if (data == null)
                    continue;

                Prewarm<LightningProjectile>(
                    data, data.prefab, data.prewarmCount
                );

                if (!pools.ContainsKey(data))
                    return false;

                RegisterVfx(data.castVfx);
                RegisterVfx(data.hitVfx);

                if (data.castVfx != null &&
                    !pools.ContainsKey(data.castVfx))
                    return false;

                if (data.hitVfx != null &&
                    !pools.ContainsKey(data.hitVfx))
                    return false;
            }
        }

        if (gameData.vfx != null)
        {
            foreach (VfxData data in gameData.vfx)
            {
                if (data == null)
                    continue;

                RegisterVfx(data);

                if (!pools.ContainsKey(data))
                    return false;
            }
        }

        // Chỉ chuẩn bị Enemy được dùng trong màn này.
        HashSet<EnemyData> registeredEnemies =
            new HashSet<EnemyData>();

        foreach (WaveSettings wave in level.waves)
        {
            foreach (EnemySpawnGroup group in wave.spawnGroups)
            {
                EnemyData data = group.enemy;

                // Một loại xuất hiện nhiều wave vẫn chỉ tạo pool một lần.
                if (!registeredEnemies.Add(data))
                    continue;

                if (data.prefab == null ||
                    data.prefab.GetComponent<EnemyHealth>() == null ||
                    data.prefab.GetComponent<Rigidbody2D>() == null)
                {
                    Debug.LogError(
                        $"EnemyData {data.name}: prefab thiếu " +
                        "EnemyHealth hoặc Rigidbody2D.",
                        data
                    );

                    return false;
                }

                Prewarm<EnemyChase>(
                    data, data.prefab, data.prewarmCount
                );

                if (!pools.ContainsKey(data))
                    return false;
            }
        }
        goldLoot = gameData.goldLoot;
        energyLoot = gameData.energyLoot;

        if (goldLoot == null ||
            energyLoot == null ||
            goldLoot.kind != LootKind.Gold ||
            energyLoot.kind != LootKind.EnergyShard)
        {
            Debug.LogError("GameData chưa gán đúng Gold Loot / Energy Loot.", this);
            return false;
        }

        Prewarm<LootPickup>(
            goldLoot,
            goldLoot.prefab,
            goldLoot.prewarmCount
        );

        Prewarm<LootPickup>(
            energyLoot,
            energyLoot.prefab,
            energyLoot.prewarmCount
        );

        if (!pools.ContainsKey(goldLoot) ||
            !pools.ContainsKey(energyLoot))
        {
            return false;
        }
        IsInitialized = true;
        return true;
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
        enemy.PrepareSpawn(data, player, position);

        item.GetComponent<EnemyHealth>().ConfigureDrops(this, data);

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
    public void ConfigureLoot(
    PlayerHealth player,
    RunCurrency currency)
    {
        lootPlayer = player;
        runCurrency = currency;
    }

    public void DropEnemyLoot(EnemyData enemy, Vector3 position)
    {
        if (!IsInitialized ||
            enemy == null ||
            lootPlayer == null ||
            runCurrency == null ||
            !runCurrency.CollectionEnabled)
        {
            return;
        }

        // Energy luôn rơi.
        for (int i = 0; i < Mathf.Max(1, enemy.energyShardAmount); i++)
        {
            SpawnLoot(energyLoot, 1, Scatter(position));
        }

        float chance = Mathf.Clamp(enemy.goldDropChance, 0f, 100f);

        bool dropGold = chance >= 100f ||
            (chance > 0f && Random.value < chance / 100f);

        if (dropGold)
        {
            SpawnLoot(
                goldLoot,
                Mathf.Max(1, enemy.goldAmount),
                Scatter(position)
            );
        }
    }

    private Vector3 Scatter(Vector3 position)
    {
        Vector2 offset = Random.insideUnitCircle * 0.25f;
        return position + new Vector3(offset.x, offset.y, 0f);
    }

    private void SpawnLoot(
        LootData data,
        int amount,
        Vector3 position)
    {
        PoolItem item = Rent(
            data,
            position,
            Quaternion.identity,
            float.PositiveInfinity
        );

        if (item != null)
        {
            item.GetComponent<LootPickup>().Launch(
                data,
                amount,
                lootPlayer,
                runCurrency
            );

            return;
        }

        // Pool đầy: gộp vào vật phẩm cùng loại đang nằm gần nhất.
        LootPickup nearest = null;
        LootPickup fallback = null;
        float nearestDistance = float.PositiveInfinity;

        foreach (PoolItem existing in allItems)
        {
            if (existing == null ||
                !existing.IsRented ||
                existing.Key != data)
            {
                continue;
            }

            LootPickup pickup = existing.GetComponent<LootPickup>();

            if (pickup == null || !pickup.IsLive)
                continue;

            fallback = pickup;

            if (!pickup.CanMerge)
                continue;

            float distance =
                (pickup.transform.position - position).sqrMagnitude;

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = pickup;
            }
        }

        // Nếu tất cả đang bay, vẫn giữ giá trị bằng cách gộp.
        LootPickup receiver = nearest != null ? nearest : fallback;

        if (receiver != null)
        {
            receiver.AddAmount(amount);
        }
        else
        {
            Debug.LogError(
                $"Không có pool hoặc vật phẩm để nhận {data.name}.",
                this
            );
        }
    }
}