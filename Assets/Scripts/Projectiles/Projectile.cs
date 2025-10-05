using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected float _speed;
    [SerializeField] protected float _cleanupDelay;
    [SerializeField] protected LayerMask _contactMask;
    
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
        OnImpact(other);
    }

    protected virtual void OnImpact(Collider contact) { }

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
