using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IPilotable : IInteractable
{
    // Methods
    public bool TryEnter(PlayerController player);
    public void Exit(PlayerController player);
    public PilotableData GetPilotableData();
    public NetworkObjectReference GetNetworkObjectReference();
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