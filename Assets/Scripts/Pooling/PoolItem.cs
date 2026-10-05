using UnityEngine;

public sealed class PoolItem : MonoBehaviour
{
    public bool IsRented { get; private set; }
    public ScriptableObject Key { get; private set; }

    private CombatPoolManager owner;
    private float remainingLifetime;

    public void Initialize(
        CombatPoolManager poolOwner,
        ScriptableObject key)
    {
        owner = poolOwner;
        Key = key;
        IsRented = false;
    }

    public void BeginLease(float lifetime)
    {
        remainingLifetime = Mathf.Max(0.01f, lifetime);
        IsRented = true;
    }

    private void Update()
    {
        if (!IsRented)
            return;

        remainingLifetime -= Time.deltaTime;

        if (remainingLifetime <= 0f)
            ReturnToPool();
    }

    public void ReturnToPool()
    {
        if (!IsRented || owner == null)
            return;

        owner.Return(this);
    }

    public void EndLease()
    {
        IsRented = false;
        remainingLifetime = 0f;
    }
}