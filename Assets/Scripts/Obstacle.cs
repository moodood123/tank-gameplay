using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle : MonoBehaviour, IDamageable
{
    [SerializeField] private DamageType _damageType;
    [SerializeField] private float _maxHealth;

    [Header("Events")] 
    [SerializeField] private UnityEvent _onBreak;
    [SerializeField] private UnityEvent _onReform;
    
    private float _health;
    private bool IsIntact => _health > 0f;

    private void Start()
    {
        _health = _maxHealth;
    }
    
    public void Break()
    {
        _onBreak.Invoke();
    }

    public void Reform()
    {
        _health = _maxHealth;
        _onReform.Invoke();
    }
    
    public void TakeDamage(float damage, DamageType type = DamageType.Basic)
    {
        if (!IsIntact) return;
        if (type == _damageType)
        {
            _health -= damage;
            if (!IsIntact) Break();
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Obstacle))]
public class ObstacleEditor : Editor
{
    private Obstacle _target;

    private void OnEnable()
    {
        _target = (Obstacle)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Break")) _target.Break();
        if (GUILayout.Button("Reform")) _target.Reform();
    }
}
#endif