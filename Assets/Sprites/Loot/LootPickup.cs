using UnityEngine;

[RequireComponent(typeof(PoolItem))]
public sealed class LootPickup : MonoBehaviour
{
    private PoolItem poolItem;
    private LootData data;
    private PlayerHealth player;
    private RunCurrency currency;

    private int amount;
    private float age;
    private float speed;
    private bool attracting;
    private bool live;

    public bool CanMerge => live && !attracting;
    public bool IsLive => live;

    public void Launch(
        LootData lootData,
        int value,
        PlayerHealth target,
        RunCurrency runCurrency)
    {
        if (poolItem == null)
            poolItem = GetComponent<PoolItem>();

        data = lootData;
        player = target;
        currency = runCurrency;

        amount = value;
        age = 0f;
        speed = data.initialSpeed;
        attracting = false;
        live = true;

        gameObject.SetActive(true);
    }

    public void AddAmount(int value)
    {
        if (live && value > 0)
            amount += value;
    }

    private void Update()
    {
        if (!live ||
            player == null ||
            !player.IsAlive ||
            currency == null ||
            !currency.CollectionEnabled ||
            Time.timeScale <= 0f)
        {
            return;
        }

        age += Time.deltaTime;

        if (age < data.pickupDelay)
            return;

        Vector3 target = player.transform.position;
        target.z = transform.position.z;

        float distance = Vector2.Distance(
            transform.position,
            target
        );

        if (!attracting)
        {
            if (distance > data.attractRadius)
                return;

            attracting = true;
        }

        speed = Mathf.MoveTowards(
            speed,
            data.maxSpeed,
            data.acceleration * Time.deltaTime
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target) >
            data.collectDistance)
        {
            return;
        }

        // Khóa trước khi cộng để không nhặt hai lần.
        live = false;

        currency.Collect(data.kind, amount);
        poolItem.ReturnToPool();
    }

    private void OnDisable()
    {
        live = false;
        attracting = false;
        amount = 0;
        age = 0f;
        speed = 0f;

        data = null;
        player = null;
        currency = null;
    }
}