using System.Collections;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Station : NetworkBehaviour, IPilotable
{
    [field: SerializeField] public string StationName { get; private set; }
    [field: SerializeField] public PilotableData StationData { get; private set; }

    [Header("References")]
    [SerializeField] protected CinemachineCamera _stationCamera;
    [SerializeField] private MeshRenderer _visualReference;
    
    private NetworkVariable<NetworkObjectReference> _pilotReference =
        new NetworkVariable<NetworkObjectReference>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public bool IsOccupied => _pilotReference.Value.TryGet(out _);

    public PlayerController CurrentPilot { get; private set; }

    protected TankController _parentTank;
    protected Collider _collider;
    protected NetworkObject _no;

    protected Transform VirtualParent { get; private set; }
    
    public event IPilotable.OnAnimationTriggered onAnimationTriggered;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _no = GetComponent<NetworkObject>();
    }

    public override void OnNetworkSpawn()
    {
        _pilotReference.OnValueChanged += OnPilotChanged;

        if (IsServer) Initialize();
    }

    public override void OnNetworkDespawn()
    {
        _pilotReference.OnValueChanged -= OnPilotChanged;
    }

    protected virtual void Update() { }

    protected virtual void LateUpdate()
    {
        if (VirtualParent)
        {
            transform.position = VirtualParent.position;
            transform.rotation = VirtualParent.rotation;
        }
    }

    private void Initialize()
    {
        _pilotReference.Value = default;
    }

    private void OnPilotChanged(NetworkObjectReference oldReference, NetworkObjectReference newReference)
    {
        if (newReference.TryGet(out NetworkObject no) && no.TryGetComponent(out PlayerController player))
        {
            CurrentPilot = player;
            DisableInteraction();
        }
        else
        {
            CurrentPilot = null;
            EnableInteraction();
        }
    }

    private void EnableInteraction()
    {
        _collider.enabled = true;
        _visualReference.enabled = true;
    }

    private void DisableInteraction()
    {
        _collider.enabled = false;
        _visualReference.enabled = false;
    }
    
    public bool TryEnter(PlayerController player)
    {
        if (!IsServer) return false;
        if (IsOccupied) return false;
        
        _pilotReference.Value = player.NetworkObject;
        return true;
    }
    
    public void Exit(PlayerController player)
    {
        if (!IsServer) return;
        
        if (!_pilotReference.Value.TryGet(out NetworkObject no) || no != player.NetworkObject) return;

        _pilotReference.Value = default;
        
    }

    public void LeavePilot(PlayerController player)
    {
        if (player != CurrentPilot) return;
        ToggleInteractability(true);
        
        ReturnCamera();
        CurrentPilot = null;
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
    
    public void Interact() { }

    [ClientRpc]
    public void SetVirtualParentClientRpc(ulong tankId, int dataIndex)
    {
        StartCoroutine(SetVirtualParentWhenSafe(tankId, dataIndex));
    }

    private IEnumerator SetVirtualParentWhenSafe(ulong tankId, int dataIndex)
    {
        while (!NetworkManager.Singleton || NetworkManager.Singleton.SpawnManager == null) yield return null;
        
        NetworkObject no = null;
        while (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(tankId, out no)) yield return null;

        yield return null;
        
        if (no && no.TryGetComponent(out TankController tank))
        {
            _parentTank = tank;
            VirtualParent = tank.SetupData[dataIndex].SpawnParent;
        }
        else Debug.LogError($"Virtual parent not assigned to station: [{transform.name}]");
    }

    #region Interface Return Methods
    public GameObject GetGameObject() { return gameObject; }
    public string GetInteractionPrompt() { return StationName; }
    public PilotableData GetPilotableData() { return StationData; }
    public NetworkObjectReference GetNetworkObjectReference() { return new NetworkObjectReference(NetworkObject); }
    #endregion

    public virtual void OnInputRelayed(InputAction.CallbackContext context) { }
}
