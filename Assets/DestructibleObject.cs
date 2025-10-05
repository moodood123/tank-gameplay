using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    [SerializeField] private List<GameObject> _shards = new List<GameObject>();

    [SerializeField] private GameObject _wholeObject;
    [SerializeField] private GameObject _brokenObject;

    private GameObject _brokenInstance;
    
    public void Break()
    {
        _wholeObject.SetActive(false);

        _brokenInstance = Instantiate(_brokenObject, _brokenObject.transform.parent);
        _brokenInstance.SetActive(true);
    }

    public void Reform()
    {
        _wholeObject.SetActive(true);

        if (_brokenInstance) Destroy(_brokenInstance);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DestructibleObject))]
public class DestructibleObjectEditor : Editor
{
    private DestructibleObject _target;

    private void OnEnable()
    {
        _target = (DestructibleObject)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Break")) _target.Break();
        if (GUILayout.Button("Reform")) _target.Reform();
    }
}
#endif