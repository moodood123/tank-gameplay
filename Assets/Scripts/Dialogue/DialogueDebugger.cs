using UnityEditor;
using UnityEngine;

public class DialogueDebugger : MonoBehaviour
{
    [SerializeField] private DialogueStatement _testStatement;

    public void TestDialogue()
    {
        if (DialogueManager.Instance) DialogueManager.Instance.PlayDialogue(_testStatement);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DialogueDebugger))]
public class DialogueDebuggerEditor : Editor
{
    private DialogueDebugger _target;

    private void OnEnable()
    {
        _target = (DialogueDebugger)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Test Dialogue")) _target.TestDialogue();
    }
}
#endif