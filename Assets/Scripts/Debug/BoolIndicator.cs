using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BoolIndicator : MonoBehaviour
{
    [SerializeField] private GameObject _trueIndicator;
    [SerializeField] private GameObject _falseIndicator;

    private bool _value;
    
    public void SetBool(bool value)
    {
        _value = value;
        _trueIndicator.SetActive(_value);
        _falseIndicator.SetActive(!_value);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BoolIndicator))]
public class BoolIndicatorEditor : Editor
{
    private BoolIndicator _target;

    private void Awake()
    {
        _target = (BoolIndicator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Set True"))
        {
            _target.SetBool(true);
        }

        if (GUILayout.Button("Set False"))
        {
            _target.SetBool(false);
        }
    }
}
#endif