using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    [SerializeField] private bool _doOnce = false;
    
    [SerializeField] private UnityEvent _onTriggerEnter;
    [SerializeField] private UnityEvent _onTriggerExit;

    private bool _isTriggered = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (_doOnce && _isTriggered) return;
        
        if (other.TryGetComponent<TankController>(out TankController tankController))
        {
            _isTriggered = true;
            _onTriggerEnter?.Invoke();
        }
    }
}
