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
    [SerializeField] private Transform _virtualParent;

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

        Debug.Log("IsOwner: " + IsOwner);
        
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
        if (_virtualParent.parent == transform) _virtualParent.parent = null;
    }

    private void Update()
    {
        CheckForInteractables();

        transform.position = _virtualParent.position;
        transform.rotation = _virtualParent.rotation;
    }

    private void TryInteract()
    {
        if (_currentPickup != null && !_currentPickup.GetGameObject().activeInHierarchy)
        {
            _currentPickup = null;
        }
        
        if (_currentInteractable == null) return;
        
        // Check for pilotable
        if (_currentInteractable is IPilotable pilotable && _canMove && pilotable.TryEnterPilot(this))
        {
            StartCoroutine(MoveToStation(pilotable));
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

    public bool Initialize(IPilotable pilotable)
    {
        bool succeeded = pilotable.TryEnterPilot(this);

        if (succeeded)
        {
            StartCoroutine(MoveToStation(pilotable));
        }

        return succeeded;
    }

    private IEnumerator MoveToStation(IPilotable pilotable)
    {
        // Disable movement
        _canMove = false;
        
        // Leave the current station
        _currentPilotable?.LeavePilot(this);
        if (_currentPilotable != null) _currentPilotable.onAnimationTriggered -= OnAnimationTriggered;
        
        // Lerp to the new station
        //transform.parent = pilotable.GetPilotableData().PilotPosition;
        _virtualParent.parent = pilotable.GetPilotableData().PilotPosition;
        yield return Tween.LocalPosition(_virtualParent, Vector3.zero, _transitionSettings).ToYieldInstruction();
        
        // Enter the new station
        _currentPilotable = pilotable;
        if (_currentPilotable != null) _currentPilotable.onAnimationTriggered += OnAnimationTriggered;
        _virtualParent.localPosition = Vector3.zero;
        
        // Re-enable movement
        _canMove = true;
    }

    private void OnAnimationTriggered(string animationTrigger)
    {
        _pa.Animate(animationTrigger);
    }

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
}
