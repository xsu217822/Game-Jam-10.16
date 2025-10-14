// Assets/Scripts/Combat/IDamageable.cs
public interface IDamageable
{
    bool IsDead { get; }
    void TakeDamage(float amount);
}

