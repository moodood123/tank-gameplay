using System;
using System.Collections;
using UnityEngine;

public class GunMount : MonoBehaviour
{
    [SerializeField] private Transform _gunPivot;
    [SerializeField] private float _matchRange;
    
    [Header("Aim Constraints")]
    [SerializeField] private Vector2 _yAimConstraint = new Vector2();
    [SerializeField] private Vector2 _xAimConstraint = new Vector2();
    
    
    private Transform _target;
    private Coroutine _aimLoop;

    private IEnumerator AimLoop()
    {
        while (true)
        {
            AimAt(_target.position + _target.forward * _matchRange);
            yield return null;
        }
    }
    
    public void Activate(Transform aimParallel)
    {
        if (_aimLoop != null) return;
        
        _target = aimParallel;
        _aimLoop = StartCoroutine(AimLoop());
    }

    public void Deactivate()
    {
        if (_aimLoop == null) return;

        StopCoroutine(_aimLoop);
        _aimLoop = null;
    }
    
    private void AimAt(Vector3 target)
    {
        _gunPivot.LookAt(target);

        float x = NormalizeAngle(_gunPivot.localRotation.eulerAngles.x);
        float y = NormalizeAngle(_gunPivot.localRotation.eulerAngles.y);
        
        x = Mathf.Clamp(x, _xAimConstraint.x, _xAimConstraint.y);
        y = Mathf.Clamp(y, _yAimConstraint.x, _yAimConstraint.y);

        _gunPivot.localRotation = Quaternion.Euler(x, y, 0);
    }
    
    private float NormalizeAngle(float angle)
    {
        angle = angle % 360;
        if (angle > 180)
            angle -= 360;
        return angle;
    }
}
