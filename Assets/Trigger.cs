using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    [SerializeField] private UnityEvent _onTriggerEnter;
    [SerializeField] private UnityEvent _onTriggerExit;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<TankController>(out TankController tankController))
        {
            _onTriggerEnter?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<TankController>(out TankController tankController))
        {
            _onTriggerExit?.Invoke();
        }
    }
}
