using System.Collections;
using PrimeTween;
using UnityEngine;

public class TestTarget : MonoBehaviour, IDamageable
{
    [SerializeField] private float _maxHealth;
    [SerializeField] private Transform _mesh;
    
    [Header("Animation Settings")] 
    [SerializeField] private TweenSettings<Vector3> _deflateSettings;
    [SerializeField] private TweenSettings<Vector3> _inflateSettings;
    
    public float Health { get; private set; }

    public bool IsAlive => Health > 0f;

    private void Start()
    {
        Health = _maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (!IsAlive) return;
        Health -= damage;
        if (!IsAlive) Die();
    }

    private void Die()
    {
        StartCoroutine(RegenSequence());
    }

    private IEnumerator RegenSequence()
    {
        yield return Tween.Scale(_mesh, _deflateSettings).ToYieldInstruction();
        yield return Tween.Scale(_mesh, _inflateSettings).ToYieldInstruction();
        Health = _maxHealth;
    }
}