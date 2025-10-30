using UnityEngine;
using UnityEngine.InputSystem;

public class GunnerStation : Station
{
    [SerializeField] private Turret _turret;
    [SerializeField] private PowerRelay _relay;

    [Header("Settings")] 
    [SerializeField] private PowerChannel _channel;
    [SerializeField] private Vector2 _turretSpeed = new Vector2();
    [SerializeField] private Vector2 _tiltConstraints = new Vector2();
    [SerializeField] private Vector2 _panConstraints = new Vector2();
    [SerializeField] private bool _wrapXRotation;
    [SerializeField] private bool _wrapYRotation;

    private Vector2 _moveInput;
    private Vector2 _turretRotation;

    public override void OnNetworkSpawn()
    {
        //_turret = VirtualParent.GetComponentInParent<Turret>();
        //_relay = VirtualParent.GetComponentInParent<PowerRelay>();

        //if (!_turret) Debug.LogError("No turret found");
        //if (!_relay) Debug.LogError("No relay found");
    }
    
    protected override void Update()
    {
        base.Update();
        return;
        HandleInput();
        HandleOrientation();
    }

    private void HandleOrientation()
    {
        Debug.Log(_turretRotation);
        if (_turret) _turret.SetRotation(_turretRotation);
    }

    private void HandleInput()
    {
        //if (_relay.PowerRatios[_channel] <= 0f) return;
        
        float x = _turretRotation.x + _moveInput.x * _turretSpeed.x * Time.deltaTime;
        float y = _turretRotation.y + _moveInput.y * _turretSpeed.y * Time.deltaTime;
        
        if (x < _tiltConstraints.x)
            x = _wrapXRotation ? _tiltConstraints.y : _tiltConstraints.x;
        else if (x > _tiltConstraints.y)
            x = _wrapXRotation ? _tiltConstraints.x : _tiltConstraints.y;

        if (y < _panConstraints.x)
            y = _wrapYRotation ? _panConstraints.y : _panConstraints.x;
        else if (y > _panConstraints.y)
            y = _wrapYRotation ? _panConstraints.x : _panConstraints.y;

        _turretRotation = new Vector2(x, y);
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
