using UnityEngine;

public class Dashboard : MonoBehaviour
{
    [SerializeField] private TankMovement _tankMovement;

    [SerializeField] private ValueIndicator _speedIndicator;
    [SerializeField] private ValueIndicator _throttleIndicator;
    [SerializeField] private ValueIndicator _gearIndicator;
    
    private void OnEnable()
    {
        _tankMovement.onBroadcastSpeed += UpdateSpeed;
        _tankMovement.onBroadcastThrottle += UpdateThrottle;
        _tankMovement.onBroadcastCurrentGear += UpdateGear;
    }

    private void OnDisable()
    {
        _tankMovement.onBroadcastSpeed -= UpdateSpeed;
        _tankMovement.onBroadcastThrottle -= UpdateThrottle;
        _tankMovement.onBroadcastCurrentGear -= UpdateGear;
    }

    private void UpdateSpeed(float speed) => _speedIndicator.SetValue(speed.ToString("000.00"));
    private void UpdateThrottle(float throttle) => _throttleIndicator.SetValue(throttle.ToString("000.00"));
    private void UpdateGear(Gear gear) => _gearIndicator.SetValue(gear.Type.ToString());
}
