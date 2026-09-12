public interface IHealth
{
    float CurrentHealth { get; }
    float MaxHealth { get; }
    bool IsAlive { get; }
}
