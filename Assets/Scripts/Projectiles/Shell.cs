using UnityEngine;

public class Shell : Projectile
{
    [SerializeField] private float _impactDamage;
    [SerializeField] private float _explosionDamage;
    [SerializeField] private float _explosionRadius;

    protected override void OnImpact(Collider contact)
    {
        base.OnImpact(contact);
        if (contact.TryGetComponent(out IDamageable damageable)) damageable.TakeDamage(_impactDamage, _damageType);
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRadius, _contactMask);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out IDamageable element))
            {
                element.TakeDamage(_explosionDamage);
            }
        }
    }
}
