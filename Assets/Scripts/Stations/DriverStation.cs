using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DriverStation : Station
{
    [Header("Settings")]
    [SerializeField] private float _moveThrottleAcceleration;
    [SerializeField] private float _moveThrottleDecay;
    [SerializeField] private Vector2 _moveThrottleRange = new Vector2();

    [SerializeField] private float _turnThrottleAcceleration;
    [SerializeField] private float _turnThrottleDecay;
    [SerializeField] private Vector2 _turnThrottleRange = new Vector2();
    
    private float _moveThrottle;
    private float _turnThrottle;

    private Vector2 _moveInput = new Vector2();
    private bool _isClutchIn = false;

    public delegate void OnInputRelay(float moveThrottle, float turnThrottle);
    public event OnInputRelay onMovementRelay;

    public delegate void OnChangeGear(GearType newGear);
    public event OnChangeGear onChangeGear;

    private void Start()
    {
        onChangeGear?.Invoke(GearType.N);
    }
    
    private void Update()
    {
        _moveThrottle += _moveInput.y * _moveThrottleAcceleration * Time.deltaTime;
        if (_moveInput.y == 0f) _moveThrottle -= _moveThrottleDecay * Time.deltaTime;
        _moveThrottle = Mathf.Clamp(_moveThrottle, _moveThrottleRange.x, _moveThrottleRange.y);

        _turnThrottle += _moveInput.x * _turnThrottleAcceleration * Time.deltaTime;
        if (_moveInput.x == 0f)
        {
            if (_turnThrottle > 0.1f)
            {
                _turnThrottle -= _turnThrottleDecay * Time.deltaTime;
            }
            else if (_turnThrottle < 0.1f)
            {
                _turnThrottle += _turnThrottleDecay * Time.deltaTime;
            }
        }
        
        _turnThrottle = Mathf.Clamp(_turnThrottle, _turnThrottleRange.x, _turnThrottleRange.y);
        
        onMovementRelay?.Invoke(_moveThrottle, _turnThrottle);
    }

    private void TryChangeGear(GearType newGearType)
    {
        if (_isClutchIn)
        {
            onChangeGear?.Invoke(newGearType);
        }
        else
        {
            Debug.LogWarning("Can't change gear");
        }
    }
    
    public override void OnInputRelayed(InputAction.CallbackContext context)
    {
        switch (context.action.name)
        {
            case "Move":
                _moveInput = context.ReadValue<Vector2>();
                break;
            case "Neutral":
                if (context.action.IsPressed()) TryChangeGear(GearType.N);
                break;
            case "Reverse":
                if (context.action.IsPressed()) TryChangeGear(GearType.R);
                break;
            case "Gear1":
                if (context.action.IsPressed()) TryChangeGear(GearType.One);
                break;
            case "Gear2":
                if (context.action.IsPressed()) TryChangeGear(GearType.Two);
                break;
            case "Gear3":
                if (context.action.IsPressed()) TryChangeGear(GearType.Three);
                break;
            case "Clutch":
                _isClutchIn = context.action.IsPressed();
                break;
        }
    }
}

[Serializable]
public class Gear
{
    [field: SerializeField] public GearType Type { get; private set; }
    [field: SerializeField] public float SpeedFactor { get; private set; }
}

public enum GearType
{
    N,
    R,
    One,
    Two,
    Three
}
