using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunnerStation : Station
{
    [Header("Settings")] 
    [SerializeField] private PowerChannel _channel;
    [SerializeField] private Vector2 _turretSpeed = new Vector2();
    [SerializeField] private Vector2 _tiltConstraints = new Vector2();
    [SerializeField] private Vector2 _panConstraints = new Vector2();
    [SerializeField] private bool _wrapXRotation;
    [SerializeField] private bool _wrapYRotation;

    private Vector2 _moveInput;
    private Vector2 _turretRotation;

    public delegate void OnTurretRotationChanged(Vector2 turretRotation);
    public event OnTurretRotationChanged onTurretRotationChanged;
    
    protected override void Update()
    {
        base.Update();
        HandleInput();
        HandleOrientation();
    }

    private void HandleOrientation()
    {
        Debug.Log(_turretRotation);

        bool isControllingClient = IsClient && CurrentPilot && _parentTank.NetworkManager.LocalClientId == CurrentPilot.OwnerClientId;
        
        if (isControllingClient)
        {
            if (IsServer)
            {
                onTurretRotationChanged?.Invoke(_turretRotation);
            }
            else
            {
                HandleOrientationServerRpc(_turretRotation);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void HandleOrientationServerRpc(Vector2 turretRotation, ServerRpcParams rpcParams = default)
    {
        if (!CurrentPilot) return;
        if (rpcParams.Receive.SenderClientId != CurrentPilot.OwnerClientId) return;

        onTurretRotationChanged?.Invoke(turretRotation);
    }

    private void HandleInput()
    {
        // TODO: Reintroduce power mechanics to turret control
        //if (_relay.PowerRatios[_channel] <= 0f) return;
        
        float pan = _turretRotation.x + _moveInput.x * _turretSpeed.x * Time.deltaTime;
        float tilt = _turretRotation.y + _moveInput.y * _turretSpeed.y * Time.deltaTime;
        
        if (pan < _tiltConstraints.x)
            pan = _wrapXRotation ? _tiltConstraints.y : _tiltConstraints.x;
        else if (pan > _tiltConstraints.y)
            pan = _wrapXRotation ? _tiltConstraints.x : _tiltConstraints.y;

        if (tilt < _panConstraints.x)
            tilt = _wrapYRotation ? _panConstraints.y : _panConstraints.x;
        else if (tilt > _panConstraints.y)
            tilt = _wrapYRotation ? _panConstraints.x : _panConstraints.y;

        _turretRotation = new Vector2(pan, tilt);
    }
    
    public override void OnInputRelayed(InputAction.CallbackContext context)
    {
        switch (context.action.name)
        {
            case "Move":
                _moveInput = context.ReadValue<Vector2>();
                break;
        }
    }
}
