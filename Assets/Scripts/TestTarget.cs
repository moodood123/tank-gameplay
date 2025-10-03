using UnityEngine;

public class TestTarget : MonoBehaviour, IDamageable
{
    [SerializeField] private float _maxHealth;
    
    public float Health { get; private set; }

    public bool IsAlive => Health > 0f;

    private void Start()
    {
        Health = _maxHealth;
    }
    
    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (!IsAlive) Die();
    }

    private void Die()
    {
        Destroy(this.gameObject);
    }
}
