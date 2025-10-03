using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private Transform _cannonPanPivot;
    [SerializeField] private Transform _cannonTiltPivot;
    
    public void SetRotation(Vector2 rotation)
    {
        _cannonTiltPivot.localRotation = Quaternion.Euler(new Vector3(rotation.y, 0f, 0f));
        _cannonPanPivot.localRotation = Quaternion.Euler(new Vector3(0f, rotation.x, 0f));
    }
}
