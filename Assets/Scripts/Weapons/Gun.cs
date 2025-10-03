using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    [SerializeField] private GameObject _projectile;
    [SerializeField] private Transform _spawnPoint;

    public bool IsFiring { get; private set; } = false;

    public virtual void OnStartFire(bool debug = false)
    {
        if (IsFiring) return;
        IsFiring = true;
    }

    public virtual void OnStopFire()
    {
        if (!IsFiring) return;
        IsFiring = false;
    }
    
    protected void Fire()
    {
        GameObject go = Instantiate(_projectile.gameObject, _spawnPoint.position, _spawnPoint.rotation);
        if (go.TryGetComponent(out Projectile projectile))
        {
            projectile.Fire();
        }
        else
        {
            Destroy(go);
            Debug.LogError("Invalid projectile prefab detected");
        }
    }
}
