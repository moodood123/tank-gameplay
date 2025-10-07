using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected float _speed;
    [SerializeField] protected float _cleanupDelay;
    [SerializeField] protected LayerMask _contactMask;
    [SerializeField] protected DamageType _damageType;
    
    private Rigidbody _rb;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    
    public void Fire()
    {
        _rb.linearVelocity = transform.forward * _speed;
        StartCoroutine(Lifespan());
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter: " + other.gameObject.name);
        OnImpact(other);
    }

    protected virtual void OnImpact(Collider contact)
    {
        Debug.Log("OnImpact: " + contact.gameObject.name);
    }

    private IEnumerator Lifespan()
    {
        yield return new WaitForSeconds(_cleanupDelay);
        Cleanup();
    }

    private void Cleanup()
    {
        Destroy(gameObject);
    }
}
