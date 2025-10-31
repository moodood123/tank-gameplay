using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TankController : NetworkBehaviour
{
    [SerializeField] private float _turnSpeed;
    [SerializeField] private float _moveSpeed;

    [Header("Module References")] 
    [SerializeField] private Turret _turret;
    
    [Header("Network Setup References")] 
    [SerializeField] private PlayerSpawner _playerSpawner;
    [field: SerializeField] public List<StationSetupData> SetupData { get; private set; } = new List<StationSetupData>();
    
    [Header("Gear Settings")] 
    [SerializeField] private List<Gear> _gears = new List<Gear>();

    private Gear _currentGear;
    private Vector2 _input;

    private DriverStation _driverStation;
    private GunnerStation _gunnerStation;

    private List<Station> _stations = new List<Station>();
    
    private Rigidbody _rb;
    private NetworkObject _no;
    
    public delegate void OnBroadcastFloat(float value);
    public event OnBroadcastFloat onBroadcastThrottle;
    public event OnBroadcastFloat onBroadcastSpeed;
        
    public delegate void OnBroadcastGear(Gear gear);
    public event OnBroadcastGear onBroadcastCurrentGear;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _no = GetComponent<NetworkObject>();
        _currentGear = _gears[0];
        
        Debug.Log($"[Server:{IsServer}] TankController Awake | NetworkObjectId: {(_no ? _no.NetworkObjectId : 0)} | InstanceID: {GetInstanceID()} | name: {name}");
    }
    
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        // Spawn tank stations
        foreach (StationSetupData data in SetupData)
        {
            GameObject go = Instantiate(data.Prefab, data.SpawnParent);

            if (go.TryGetComponent(out Station station) && go.TryGetComponent(out NetworkObject no))
            {
                no.Spawn(true);
                
                int index = SetupData.IndexOf(data);
                station.SetVirtualParentClientRpc(this.NetworkObjectId, index);
            }

            _stations.Add(station);

            if (station is DriverStation driverStation)
            {
                _driverStation = driverStation;
                _driverStation.onChangeGear += SetGear;
                _driverStation.onMovementRelay += OnMovementRelay;
            }

            if (station is GunnerStation gunnerStation)
            {
                _gunnerStation = gunnerStation;
                _gunnerStation.onTurretRotationChanged += SetTurretRotation;
            }
        }
        
        // Spawn players
        _playerSpawner.SpawnInitialPlayers(_stations);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;

        if (_driverStation)
        {
            _driverStation.onChangeGear -= SetGear;
            _driverStation.onMovementRelay -= OnMovementRelay;
        }

        if (_gunnerStation)
        {
            _gunnerStation.onTurretRotationChanged -= SetTurretRotation;
        }
}

    private void Update()
    {
        if (IsServer)
        {
            BroadcastSpeedClientRpc(_rb.linearVelocity.magnitude);
            BroadcastThrottleClientRpc(_input.x);
        }
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            _rb.AddForce(transform.forward * _input.x * _currentGear.SpeedFactor * _moveSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
            _rb.MoveRotation(Quaternion.Euler(transform.rotation.eulerAngles + transform.up * _input.y * _turnSpeed * Time.fixedDeltaTime));
        }
    }

    private void SetTurretRotation(Vector2 input)
    {
        // TODO: Add broadcast interval with smoothing system in receiving classes
        if (IsServer) SetTurretRotationClientRpc(input);
    }

    [ClientRpc]
    private void SetTurretRotationClientRpc(Vector2 input)
    {
        _turret.SetRotation(input);
    }

    [ClientRpc]
    private void BroadcastSpeedClientRpc(float value)
    {
        onBroadcastSpeed?.Invoke(value);
    }

    [ClientRpc]
    private void BroadcastThrottleClientRpc(float value)
    {
        onBroadcastThrottle?.Invoke(value);
    }

    private void SetGear(GearType newGear)
    {
        if (TryGetGear(newGear, out Gear gear))
        {
            _currentGear = gear;
            BroadcastCurrentGearClientRpc(_gears.IndexOf(gear));
        }
        else
        {
            Debug.LogWarning("No gear found for type " + newGear);
        }
    }

    [ClientRpc]
    private void BroadcastCurrentGearClientRpc(int index)
    {
        onBroadcastCurrentGear?.Invoke(_gears[index]);
    }

    private void OnMovementRelay(float moveInput, float turnInput)
    {
        _input = new Vector2(moveInput, turnInput);
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
}

[Serializable]
public class StationSetupData
{
    [field: SerializeField] public Transform SpawnParent { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
}