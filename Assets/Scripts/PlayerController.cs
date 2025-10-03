using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _interactRange = 10f;
    [field: SerializeField] public Transform CameraTransform { get; private set; }
    [SerializeField] private Transform _handTransform;
    [SerializeField] private LayerMask _interactionMask;

    [Header("Pause References")]
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private CinemachinePanTilt _panTilt;
    
    public bool IsPaused { get; private set; } = false;
    
    private IInteractable _currentInteractable;
    private IPilotable _currentPilotable;
    private IPickup _currentPickup;
    private IReceiver _currentReceiver;
    private PlayerInput _pi;
    
    public delegate void OnInteractableChanged(IInteractable newInteractable);
    public event OnInteractableChanged onInteractableChanged;

    public delegate void OnPause();
    public static event OnPause onPause;

    private void Awake()
    {
        _pi = GetComponent<PlayerInput>();
    }
    
    private void OnEnable()
    {
        _pi.onActionTriggered += OnInputReceived;
    }

    private void OnDisable()
    {
        _pi.onActionTriggered -= OnInputReceived;
    }

    private void Start()
    {
        HideCursor();
    }

    private void Update()
    {
        CheckForInteractables();
    }

    private void TryInteract()
    {
        if (_currentPickup != null && !_currentPickup.GetGameObject().activeInHierarchy)
        {
            Debug.LogWarning("Held item is invalid or destroyed. Clearing.");
            _currentPickup = null;
        }
        
        
        
        Debug.Log("Trying interaction");
        if (_currentInteractable == null) return;
        Debug.Log("Interacting...");
        
        // Check for pilotable
        if (_currentInteractable is IPilotable pilotable && pilotable.TryPilot(this))
        {
            Debug.Log("Pilot successful");
            MoveToStation(pilotable);
        }
        
        // Check for pickup
        if (_currentInteractable is IPickup item && _currentPickup == null && item.TryPickup(_handTransform, out item))
        {
            Debug.Log("Pickup successful");
            _currentPickup = item;
        }
        
        // Check for receiver
        if (_currentInteractable is IReceiver receiver)
        {
            if (_currentPickup != null && receiver.IsItemCompatible(_currentPickup) && receiver.TryPlaceItem(_currentPickup))
            {
                Debug.Log("Placement successful");
                _currentPickup = null;
            }
            else if (_currentPickup == null && receiver.TryCollectItem(_handTransform, out IPickup potential))
            {
                Debug.Log("Pickup successful");
                _currentPickup = potential;
            }
        }
        
        // Base interaction
        _currentInteractable.Interact();

        CheckForInteractables();
    }

    private void MoveToStation(IPilotable pilotable)
    {
        transform.parent = pilotable.GetPilotableData().PilotPosition;
        transform.localPosition = Vector3.zero;
        if (_currentPilotable != null) _currentPilotable.LeavePilot(this);
        _currentPilotable = pilotable;
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
        IsPaused = !IsPaused;

        _pauseMenu.SetActive(IsPaused);
        _panTilt.enabled = !IsPaused;

        if (!IsPaused) HideCursor();
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
