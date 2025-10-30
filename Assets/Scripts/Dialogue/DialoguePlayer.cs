using UnityEditor;
using UnityEngine;

public class DialoguePlayer : MonoBehaviour
{
    [SerializeField] private DialogueStatement _statement;

    public void PlayDialogue()
    {
        if (DialogueManager.Instance)
        {
            DialogueManager.Instance.PlayDialogue(_statement);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DialoguePlayer))]
public class DialoguePlayerEditor : Editor
{
    private DialoguePlayer _target;

    private void OnEnable()
    {
        _target = (DialoguePlayer)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Player Dialogue")) _target.PlayDialogue();
    }
}
#endif