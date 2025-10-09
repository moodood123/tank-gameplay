using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Station : NetworkBehaviour, IPilotable
{
    [field: SerializeField] public string StationName { get; private set; }
    [field: SerializeField] public PilotableData StationData { get; private set; }

    [SerializeField] protected CinemachineCamera _stationCamera;
    
    protected PlayerController _pilot;
    protected Collider _collider;
    
    public event IPilotable.OnAnimationTriggered onAnimationTriggered;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }
    
    public bool TryEnterPilot(PlayerController player)
    {
        if (!_pilot)
        {
            _pilot = player;
            ToggleInteractability(false);
            
            OverrideCamera();
            return true;
        }
        return false;
    }

    public void LeavePilot(PlayerController player)
    {
        if (player != _pilot) return;
        ToggleInteractability(true);
        
        ReturnCamera();
        _pilot = null;
        Debug.Log("Pilot left station");
    }

    protected void AnimatePlayer(string animationKey)
    {
        onAnimationTriggered?.Invoke(animationKey);
    }

    protected void ToggleInteractability(bool isInteractable)
    {
        _collider.enabled = isInteractable;
    }

    protected void OverrideCamera()
    {
        _stationCamera.Priority = 100;
    }

    protected void ReturnCamera()
    {
        _stationCamera.Priority = -100;
    }
    
    protected virtual void OnPlayerEnter() { }
    protected virtual void OnPlayerLeave() { }
    
    public string GetInteractionPrompt() { return StationName; }
    
    public void Interact() { }
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public PilotableData GetPilotableData() { return StationData; }

    public virtual void OnInputRelayed(InputAction.CallbackContext context) { }
}
