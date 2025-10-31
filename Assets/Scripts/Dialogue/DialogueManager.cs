using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-10000)]
public class DialogueManager : MonoBehaviour
{
    private DialogueStatement _ongoingStatement;
    private Coroutine _statementCoroutine;
    
    public static DialogueManager Instance { get; private set; }

    public delegate void OnDisplaySubtitleText(string text);
    public event OnDisplaySubtitleText onDisplaySubtitleText;
    
    public delegate void OnPlayAudio(string reference);
    public event OnPlayAudio onPlayAudio;
    
    private void Awake()
    {
        // Initialize singleton
        if (!Instance)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void PlayDialogue(DialogueStatement statement)
    {
        Debug.Log("Playing Dialogue");
        
        if (_ongoingStatement != null && statement.OverrideType != StatementOverrideType.Override) return;

        Debug.LogWarning("Playing Dialogue");
        if (_statementCoroutine != null)
        {
            StopCoroutine(_statementCoroutine);
            CleanupDialogue();
        }

        _ongoingStatement = statement;
        _statementCoroutine = StartCoroutine(DialogueSequence(statement));
    }

    private IEnumerator DialogueSequence(DialogueStatement statement)
    {
        for (int i = 0; i < statement.ValueSequence.Count; i++)
        {
            onDisplaySubtitleText?.Invoke(statement.ValueSequence[i]);
            if (statement.AudioReferenceSequence.Count > i) onPlayAudio?.Invoke(statement.AudioReferenceSequence[i]);
            if (statement.DurationSequence.Count > i && statement.DurationSequence[i] > 0f) yield return new WaitForSeconds(statement.DurationSequence[i]);
            else yield return null;
        }

        CleanupDialogue();
        _ongoingStatement = null;
    }

    private void CleanupDialogue()
    {
        onDisplaySubtitleText?.Invoke(string.Empty);
        onPlayAudio?.Invoke(string.Empty);
    }
}

[Serializable]
public class DialogueStatement
{
    [Header("Content")]
    [field: SerializeField] public List<string> ValueSequence { get; private set; }
    [field: SerializeField] public List<string> AudioReferenceSequence { get; private set; }
    [field: SerializeField] public List<float> DurationSequence { get; private set; }
    
    [Header("Priority Settings")]
    [field: SerializeField] public StatementOverrideType OverrideType { get; private set; }
    
    public DialogueStatement(List<string> valueSequence, List<string> audioReferenceSequence, List<float> durationSequence, StatementOverrideType overrideType)
    {
        ValueSequence = valueSequence;
        AudioReferenceSequence = audioReferenceSequence;
        DurationSequence = durationSequence;
        OverrideType = overrideType;
    }
}

public enum StatementOverrideType
{
    Override,
    Concede
}