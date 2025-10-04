using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class TestAgent : AgentController
{
    [SerializeField] private float _speed = 5f;
    
    private Vector2 _input = new Vector2();
    
    private Rigidbody _rb;
    private PlayerInput _pi;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
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

    private void FixedUpdate()
    {
        _rb.linearVelocity = new Vector3(_input.x, 0f, _input.y) * _speed;
    }

    private void OnInputReceived(InputAction.CallbackContext context)
    {
        switch (context.action.name)
        {
            case "Move":
            {
                _input = context.ReadValue<Vector2>();
                break;
            }
        }
    }
}
