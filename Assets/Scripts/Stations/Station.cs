using UnityEngine;
using UnityEngine.InputSystem;

public class Station : MonoBehaviour, IPilotable
{
    [field: SerializeField] public string StationName { get; private set; }
    [field: SerializeField] public PilotableData StationData { get; private set; }

    protected PlayerController _pilot;
    
    public bool TryPilot(PlayerController player)
    {
        if (!_pilot)
        {
            _pilot = player;
            return true;
        }
        return false;
    }

    public void LeavePilot(PlayerController player)
    {
        if (player != _pilot) return;
        
        _pilot = null;
        Debug.Log("Pilot left station");
    }

    public string GetInteractionPrompt() { return StationName; }
    
    public void Interact() { }
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public PilotableData GetPilotableData() { return StationData; }

    public virtual void OnInputRelayed(InputAction.CallbackContext context) { }
}
