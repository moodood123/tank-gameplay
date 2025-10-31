using UnityEngine;

public class Compass : MonoBehaviour
{
    [SerializeField] private Transform _referenceObject;

    [SerializeField] private Transform _compassNeedle;
    
    private float Direction => GetDirection();

    private void Update()
    {
        _compassNeedle.localEulerAngles = new Vector3(0f, Direction, 0f);
    }
    
    private float GetDirection()
    {
        Vector3 referenceVector = _referenceObject.forward;
        referenceVector.y = 0;

        int sign = Vector3.Angle(_referenceObject.right, Vector3.forward) < Vector3.Angle(-_referenceObject.right, Vector3.forward) ? 1 : -1;

        return Vector3.Angle(referenceVector, Vector3.forward) * sign;
    }
}
