using UnityEngine;

public class Gauge : MonoBehaviour
{
    [SerializeField] private Vector2 _zRotationConstraint = new Vector2();
    [SerializeField] private float _maxValue = 10f;
    [SerializeField] private Transform _dial;

    private float _ratio;
    
    public void UpdateValue(float value)
    {
        _ratio = 1 - (value / _maxValue);

        float zRotation = _zRotationConstraint.x + _ratio * (_zRotationConstraint.y - _zRotationConstraint.x);

        _dial.localRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.x, zRotation);
    }
}
