using System;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IPilotable : IInteractable
{
    // Methods
    public bool TryEnterPilot(PlayerController player);
    public void LeavePilot(PlayerController player);
    public PilotableData GetPilotableData();
    public void OnInputRelayed(InputAction.CallbackContext context);
    
    // Delegates
    public delegate void OnAnimationTriggered(string animationKey);
    public event OnAnimationTriggered onAnimationTriggered;
    
}

[Serializable]
public class PilotableData
{
    [field: SerializeField] public Transform PilotPosition { get; private set; }
}