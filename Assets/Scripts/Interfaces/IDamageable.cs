using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(float damage, DamageType type = DamageType.Basic);
}

public enum DamageType
{
    Basic,
    Incendiary
}