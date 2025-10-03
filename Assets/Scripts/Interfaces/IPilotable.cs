using System;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IPilotable : IInteractable
{
    public bool TryPilot(PlayerController player);
    public void LeavePilot(PlayerController player);
    public PilotableData GetPilotableData();
    public void OnInputRelayed(InputAction.CallbackContext context);
}

[Serializable]
public class PilotableData
{
    [field: SerializeField] public Transform PilotPosition { get; private set; }
}