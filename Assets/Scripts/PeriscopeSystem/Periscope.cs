using UnityEngine;
using UnityEngine.UI;

public class Periscope : MonoBehaviour
{
    private PeriscopeRelay _relay;
    [SerializeField] private PeriscopeIndex _initialIndex;
    
    [Header("Output Settings")]
    [SerializeField] private RawImage _outputScreen;
    [SerializeField] private float _renderInterval;
    
    private void Awake()
    {
        _relay = GetComponentInParent<PeriscopeRelay>();
    }
    
    private void Start()
    {
        SetInputIndex(_initialIndex);
    }

    public void SetInputIndex(PeriscopeIndex newIndex)
    {
        if (_relay)
        {
            _outputScreen.texture = _relay.Relay[newIndex].RenderTexture;
            Material instance = Instantiate(_outputScreen.material);
            instance.SetTexture("_MainTex", _relay.Relay[newIndex].RenderTexture);
            _outputScreen.material = instance;
        }
    }
}

