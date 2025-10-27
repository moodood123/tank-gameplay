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

    [Header("Network Setup References")] 
    [SerializeField] private PlayerSpawner _playerSpawner;
    [SerializeField] private List<StationSetupData> _setupData = new List<StationSetupData>();
    
    [Header("Gear Settings")] 
    [SerializeField] private List<Gear> _gears = new List<Gear>();

    private Gear _currentGear;
    private Vector2 _input;

    private DriverStation _driverStation;

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
    }
    
    public override void OnNetworkSpawn()
    {
        Debug.Log("OnEnable: " + IsServer);
        if (!IsServer) return;

        Debug.Log("Spawning stations on network");
        foreach (StationSetupData data in _setupData)
        {
            GameObject go = Instantiate(data.Prefab, data.SpawnParent);

            if (go.TryGetComponent(out Station station) && go.TryGetComponent(out NetworkObject no))
            {
                station.SetVirtualParent(data.SpawnParent);
                
                no.Spawn(true);
            }

            _stations.Add(station);

            if (station is DriverStation driverStation) _driverStation = driverStation;
        }
        
        if (_driverStation)
        {
            _driverStation.onChangeGear += SetGear;
            _driverStation.onMovementRelay += OnMovementRelay;
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
    }

    private void Update()
    {
        onBroadcastSpeed?.Invoke(_rb.linearVelocity.magnitude);
        onBroadcastThrottle?.Invoke(_input.x);
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            _rb.AddForce(transform.forward * _input.x * _currentGear.SpeedFactor * _moveSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
            _rb.MoveRotation(Quaternion.Euler(transform.rotation.eulerAngles + transform.up * _input.y * _turnSpeed * Time.fixedDeltaTime));
        }
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