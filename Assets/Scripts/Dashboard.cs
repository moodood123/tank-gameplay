using UnityEngine;

public class Dashboard : MonoBehaviour
{
    [SerializeField] private TankController _tankController;

    [Header("Display Options")]
    [SerializeField] private ValueIndicator _speedIndicator;
    [SerializeField] private ValueIndicator _throttleIndicator;
    [SerializeField] private ValueIndicator _gearIndicator;
    [SerializeField] private Gauge _speedGauge;
    [SerializeField] private Gauge _throttleGauge;
    
    private void OnEnable()
    {
        _tankController.onBroadcastSpeed += UpdateSpeed;
        _tankController.onBroadcastThrottle += UpdateThrottle;
        _tankController.onBroadcastCurrentGear += UpdateGear;
    }

    private void OnDisable()
    {
        _tankController.onBroadcastSpeed -= UpdateSpeed;
        _tankController.onBroadcastThrottle -= UpdateThrottle;
        _tankController.onBroadcastCurrentGear -= UpdateGear;
    }

    private void UpdateSpeed(float speed)
    {
        _speedIndicator.SetValue(speed.ToString("000.00"));
        if (_speedGauge) _speedGauge.UpdateValue(speed);
    }

    private void UpdateThrottle(float throttle)
    {
        _throttleIndicator.SetValue(throttle.ToString("000.00"));
        if (_throttleGauge) _throttleGauge.UpdateValue(throttle);
    }

    private void UpdateGear(Gear gear) => _gearIndicator.SetValue(gear.Type.ToString());
}
