using UnityEngine;
using UnityEngine.UI;

public class Periscope : MonoBehaviour
{
    private PeriscopeRelay _relay;
    [SerializeField] private PeriscopeIndex _index;
    
    [Header("Output Settings")]
    [SerializeField] private RawImage _outputScreen;
    [SerializeField] private float _renderInterval;
    
    private void Awake()
    {
        _relay = GetComponentInParent<PeriscopeRelay>();
    }
    
    private void Start()
    {
        if (_relay)
        {
            _outputScreen.texture = _relay.Relay[_index].RenderTexture;
            Material instance = Instantiate(_outputScreen.material);
            instance.SetTexture("_MainTex", _relay.Relay[_index].RenderTexture);
            _outputScreen.material = instance;
        }
    }
}

