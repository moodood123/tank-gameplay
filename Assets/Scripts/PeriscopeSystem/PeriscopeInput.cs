using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class PeriscopeInput : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private PeriscopeIndex _index;
    public RenderTexture RenderTexture { get; private set; }
    private PeriscopeRelay _relay;
    
    [Header("Render Settings")] 
    [SerializeField] private float _renderInterval = 0.1f;

    private bool _isRendering = true;

    private void Awake()
    {
        Setup();
    }

    private void Start()
    {
        StartCoroutine(RenderLoop());
    }

    public void Activate()
    {
        _isRendering = true;
    }

    public void Deactivate()
    {
        _isRendering = false;
    }
    
    private IEnumerator RenderLoop()
    {
        while (true)
        {
            yield return new WaitUntil(() => _isRendering);
            UpdatePeriscope();
            yield return new WaitForSeconds(_renderInterval);
        }
    }

    private void UpdatePeriscope()
    {
        RenderQueue.Render(_camera);
    }

    private void Setup()
    {
        _relay = GetComponentInParent<PeriscopeRelay>();
        
        // Create a render texture
        RenderTexture = new RenderTexture(512, 512, 16);
        RenderTexture.Create();

        _camera.targetTexture = RenderTexture;
        _camera.enabled = false;
        
        if (_relay) _relay.AddInput(_index, this);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PeriscopeInput))]
public class PeriscopeInputEditor : Editor
{
    private PeriscopeInput _target;

    private void Awake()
    {
        _target = (PeriscopeInput)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Activate"))
        {
            _target.Activate();
        }

        if (GUILayout.Button("Deactivate"))
        {
            _target.Deactivate();
        }
    }
}
#endif
