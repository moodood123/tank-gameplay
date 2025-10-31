using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : AgentController
{
    [SerializeField] private float _interactRange = 10f;
    [field: SerializeField] public Transform CameraTransform { get; private set; }
    [SerializeField] private Transform _handTransform;
    [SerializeField] private LayerMask _interactionMask;

    [Header("General References")] 
    [SerializeField] private GameObject _playerCamera;
    [SerializeField] private GameObject _virtualCamera;
    
    [Header("Visual References")] 
    [SerializeField] private List<GameObject> _thirdPersonElements = new List<GameObject>();
    [SerializeField] private List<GameObject> _firstPersonElements = new List<GameObject>();
    [SerializeField] private int _thirdPersonLayerMask;
    [SerializeField] private int _firstPersonLayerMask;
    
    [Header("Pause References")]
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private CinemachinePanTilt _panTilt;

    [Header("Animation Settings")] 
    [SerializeField] private TweenSettings _transitionSettings;
    
    private bool _isPaused  = false;
    private bool _canMove = true;
    
    // Accessible interface references
    private IInteractable _currentInteractable;
    private IPilotable _currentPilotable;
    private IPickup _currentPickup;
    private IReceiver _currentReceiver;
    
    private PlayerInput _pi;
    private PlayerAnimation _pa;
    private NetworkObject _no;
    
    public delegate void OnInteractableChanged(IInteractable newInteractable);
    public event OnInteractableChanged onInteractableChanged;

    private void Awake()
    {
        _pi = GetComponent<PlayerInput>();
        _pa = GetComponent<PlayerAnimation>();
        _no = GetComponent<NetworkObject>();
    }
    
    private void OnEnable()
    {
        _pi.onActionTriggered += OnInputReceived;
    }

    private void OnDisable()
    {
        _pi.onActionTriggered -= OnInputReceived;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        foreach (GameObject element in _thirdPersonElements)
        {
            element.layer = IsOwner ? _thirdPersonLayerMask : _firstPersonLayerMask;
        }

        foreach (GameObject element in _firstPersonElements)
        {
            element.layer =  IsOwner ? _firstPersonLayerMask : _thirdPersonLayerMask;
        }

        _playerCamera.SetActive(IsOwner);
        _virtualCamera.SetActive(IsOwner);
        _pi.enabled = IsOwner;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }

    protected override void Start()
    {
        base.Start();
        HideCursor();
    }

    private void Update()
    {
        CheckForInteractables();
    }

    private void LateUpdate()
    {
        transform.position = _currentPilotable.GetPilotableData().PilotPosition.position;
        transform.rotation = _currentPilotable.GetPilotableData().PilotPosition.rotation;
    }

    private void TryInteract()
    {
        if (_currentPickup != null && !_currentPickup.GetGameObject().activeInHierarchy)
        {
            _currentPickup = null;
        }
        
        if (_currentInteractable == null) return;
        
        // Check for pilotable
        if (_currentInteractable is IPilotable pilotable && _canMove)
        {
            RequestEnterStationServerRpc(pilotable.GetNetworkObjectReference());
        }
        
        // Check for pickup
        if (_currentInteractable is IPickup item && _currentPickup == null && item.TryPickup(_handTransform, out item))
        {
            _currentPickup = item;
        }
        
        // Check for receiver
        if (_currentInteractable is IReceiver receiver)
        {
            if (_currentPickup != null && receiver.IsItemCompatible(_currentPickup) && receiver.TryPlaceItem(_currentPickup))
            {
                _currentPickup = null;
            }
            else if (_currentPickup == null && receiver.TryCollectItem(_handTransform, out IPickup potential))
            {
                _currentPickup = potential;
            }
        }
        
        // Base interaction
        _currentInteractable.Interact();

        CheckForInteractables();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestEnterStationServerRpc(NetworkObjectReference stationReference)
    {
        if (!stationReference.TryGet(out NetworkObject no) || !no.TryGetComponent(out Station station)) return;

        if (station.TryEnter(this))
        {
            AssignStationClientRpc(stationReference);
        }
    }

    [ServerRpc]
    private void RequestPickupItemServerRpc(NetworkObjectReference pickupReference)
    {
        if (!pickupReference.TryGet(out NetworkObject no) || !no.TryGetComponent(out IPickup pickup)) return;

        if (pickup.TryPickup(_handTransform, out IPickup item))
        {
            
        }
    }

    [ServerRpc]
    private void RequestPlaceItemServerRpc(NetworkObjectReference pickupReference)
    {
        if (!pickupReference.TryGet(out NetworkObject no) || !no.TryGetComponent(out IReceiver receiver)) return;

        if (receiver.TryPlaceItem(_currentPickup))
        {
            // TODO: Add logic for dropping items
        }
    }

    [ClientRpc]
    public void AssignStationClientRpc(NetworkObjectReference stationReference)
    {
        if (stationReference.TryGet(out NetworkObject no) && no.TryGetComponent(out IPilotable pilotable))
        {
            // Leave the current station
            _currentPilotable?.Exit(this);
            if (_currentPilotable != null) _currentPilotable.onAnimationTriggered -= OnAnimationTriggered;
            
            // Enter the new station
            _currentPilotable = pilotable;
            StartCoroutine(MoveToStation(pilotable));
        }
    }

    private IEnumerator MoveToStation(IPilotable pilotable)
    {
        // Disable movement
        _canMove = false;
        
        // Lerp to the new station
        yield return Tween.LocalPosition(pilotable.GetPilotableData().PilotPosition, Vector3.zero, _transitionSettings).ToYieldInstruction();
        
        // Enter the new station
        if (_currentPilotable != null) _currentPilotable.onAnimationTriggered += OnAnimationTriggered;
        
        // Re-enable movement
        _canMove = true;
    }

    private void OnAnimationTriggered(string animationTrigger)
    {
        _pa.Animate(animationTrigger);
    }

    #region Interaction Detection
    private void CheckForInteractables()
    {
        if (TryGetInteractable(out IInteractable interactable)) { }
        else { }
        
        if (interactable != _currentInteractable)
        {
            _currentInteractable = interactable;
            onInteractableChanged?.Invoke(_currentInteractable);
        }
    }
    
    private bool TryGetInteractable(out IInteractable interactable)
    {
        interactable = null;
        if (!Physics.Raycast(CameraTransform.position, CameraTransform.forward, out RaycastHit hit, _interactRange, _interactionMask)) return false;
        if (hit.collider.TryGetComponent(out interactable)) return true;
        return false;
    }
    
    #endregion
    
    #region UI Handlers
    public void TogglePause()
    {
        _isPaused = !_isPaused;

        _pauseMenu.SetActive(_isPaused);
        _panTilt.enabled = !_isPaused;

        if (!_isPaused) HideCursor();
        else ShowCursor();
    }
    
    private void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    #endregion
    
    private void OnInputReceived(InputAction.CallbackContext context)
    {
        _currentPilotable?.OnInputRelayed(context);

        switch (context.action.name)
        {
            case "Interact":
                if (context.action.IsPressed()) TryInteract();
                break;
            case "Pause":
                TogglePause();
                break;
        }
    }
}
