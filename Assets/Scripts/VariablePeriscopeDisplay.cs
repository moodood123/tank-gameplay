using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VariablePeriscopeDisplay : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Periscope _periscope;
    [SerializeField] private TextMeshProUGUI _descriptorText;
    
    [Header("Settings")]
    [SerializeField] private List<AccessibleInput> _accessibleInputs = new List<AccessibleInput>();

    private AccessibleInput CurrentInput => _accessibleInputs[_currentInputIndex];
    private int _currentInputIndex = 0;

    public void ScrollNext()
    {
        _currentInputIndex++;
        if (_currentInputIndex >= _accessibleInputs.Count) _currentInputIndex = 0;

        _periscope.SetInputIndex(CurrentInput.Index);
        _descriptorText.text = CurrentInput.Descriptor;
    }

    public void ScrollPrevious()
    {
        _currentInputIndex--;
        if (_currentInputIndex < 0) _currentInputIndex = _accessibleInputs.Count - 1;
        
        _periscope.SetInputIndex(CurrentInput.Index);
        _descriptorText.text = CurrentInput.Descriptor;
    }
}

[Serializable]
public class AccessibleInput
{
    [field: SerializeField] public PeriscopeIndex Index { get; private set; }
    [field: SerializeField] public string Descriptor { get; private set; }
}