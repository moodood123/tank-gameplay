using UnityEngine;

public class Bullet : Projectile
{
    [SerializeField] private float _damage;

    protected override void OnImpact(Collider contact)
    {
        if (contact.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(_damage);
        }
    }
}
