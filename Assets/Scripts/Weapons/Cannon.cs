using System.Collections;
using PrimeTween;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(CannonLoader))]
public class Cannon : Gun
{
    [Header("References")]
    [SerializeField] private Transform _barrelTransform;
    [SerializeField] private Transform _loadShellReferencePoint;
    
    [Header("Animation Settings")] 
    [SerializeField] private TweenSettings<float> _recoilSettings;
    [SerializeField] private TweenSettings<float> _recoverySettings;
    [SerializeField] private VisualEffect _shotEffect;

    public bool IsLoaded => _loader.IsLoaded;

    private CannonLoader _loader;

    private void Awake()
    {
        _loader = GetComponent<CannonLoader>();
    }
    
    public override void OnStartFire(bool debugFire = false)
    {
        if (IsLoaded)
        {
            Fire();
            _shotEffect.SendEvent("OnFire");
            StartCoroutine(RecoilSequence());
        }
        else if (debugFire)
        {
            Fire();
            _shotEffect.SendEvent("OnFire");
            StartCoroutine(RecoilSequence());
        }
    }

    private IEnumerator RecoilSequence()
    {
        yield return Tween.LocalPositionZ(_barrelTransform, _recoilSettings).ToYieldInstruction();
        _loader.EjectSpentShell();
        yield return Tween.LocalPositionZ(_barrelTransform, _recoverySettings).ToYieldInstruction();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Cannon))]
public class CannonEditor : Editor
{
    private Cannon _target;

    private void Awake()
    {
        _target = (Cannon)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Fire"))
        {
            _target.OnStartFire();
        }
    }
}
#endif