using System.Collections;
using UnityEditor;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    [Header("Debug")] 
    [SerializeField] private string _debugValue;

    public void Animate(string trigger = null)
    {
        if (trigger == null) trigger = _debugValue;
        
        _animator.SetTrigger(trigger);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerAnimation))]
public class PlayerAnimationEditor : Editor
{
    private PlayerAnimation _target;

    private void Awake()
    {
        _target = (PlayerAnimation)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Animate"))
        {
            _target.Animate();
        }
    }
}
#endif