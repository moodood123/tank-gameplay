using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TankMovement : MonoBehaviour
{
    [SerializeField] private DriverStation _driverStation;
    [SerializeField] private float _turnSpeed;
    [SerializeField] private float _moveSpeed;
    
    [Header("Gear Settings")] 
    [SerializeField] private List<Gear> _gears = new List<Gear>();
    
    private Gear _currentGear;
    private Vector2 _input;
    
    private Rigidbody _rb;

    public delegate void OnBroadcastFloat(float value);
    public event OnBroadcastFloat onBroadcastThrottle;
    public event OnBroadcastFloat onBroadcastSpeed;
        
    public delegate void OnBroadcastGear(Gear gear);
    public event OnBroadcastGear onBroadcastCurrentGear;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    
    private void OnEnable()
    {
        _driverStation.onChangeGear += SetGear;
        _driverStation.onMovementRelay += OnMovementRelay;
    }

    private void OnDisable()
    {
        _driverStation.onChangeGear -= SetGear;
        _driverStation.onMovementRelay -= OnMovementRelay;
    }

    private void Update()
    {
        onBroadcastSpeed(_rb.linearVelocity.magnitude);
        onBroadcastThrottle(_input.x);
    }

    private void FixedUpdate()
    {
        _rb.AddForce(transform.forward * _input.x * _currentGear.SpeedFactor * _moveSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
        
        _rb.MoveRotation(Quaternion.Euler(transform.rotation.eulerAngles + transform.up * _input.y * _turnSpeed * Time.fixedDeltaTime));
    }

    private void SetGear(GearType newGear)
    {
        if (TryGetGear(newGear, out Gear gear))
        {
            _currentGear = gear;
            onBroadcastCurrentGear?.Invoke(gear);
        }
        else
        {
            Debug.LogWarning("No gear found for type " + newGear);
        }
    }
    
    private bool TryGetGear(GearType type, out Gear result)
    {
        foreach (Gear gear in _gears)
        {
            if (gear.Type == type)
            {
                result = gear;
                return true;
            }
        }
        result = null;
        return false;
    }

    private void OnMovementRelay(float moveInput, float turnInput)
    {
        _input = new Vector2(moveInput, turnInput);
    }
}
