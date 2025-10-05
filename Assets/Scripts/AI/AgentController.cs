using UnityEngine;

public class AgentController : MonoBehaviour, IDamageable
{
    [field: SerializeField] public Team AgentTeam { get; private set; }
    
    [SerializeField] protected float _maxHealth;

    protected float _health;

    public bool IsAlive => _health > 0f;

    public delegate void OnAgentDeath(AgentController agentController);
    public event OnAgentDeath onAgentDeath;
    
    protected virtual void Start()
    {
        _health = _maxHealth;
    }
    
    public void TakeDamage(float damage, DamageType type)
    {
        Debug.Log("TakeDamage");
        if (!IsAlive) return;
        
        _health -= damage;
        if (!IsAlive) Die();
    }
    
    protected virtual void Die() { }
}

public enum Team
{
    Player,
    Enemy
}