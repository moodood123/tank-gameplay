using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FixedCannon : AgentController
{
    [Header("References")] 
    [SerializeField] private Gun _cannon;
    [SerializeField] private Transform _panPivot;
    [SerializeField] private Transform _tiltPivot;
    [SerializeField] private Transform _aimReference;
    [SerializeField] private BoolIndicator _lifeIndicator;
    [SerializeField] private AgentDetector _detector;

    [Header("Settings")] 
    [SerializeField] private float _panSpeed;
    [SerializeField] private float _tiltSpeed;
    [SerializeField] private float _trackingFireDelay = 5f;
    [SerializeField] private float _aimAffordance = 5f;
    [SerializeField] private float _reloadDelay = 2f;

    [Header("Debug")] 
    [SerializeField] private Transform _debugTarget;
    
    private Transform _trackingTarget;

    private bool _isLoaded = true;

    private bool HasTarget => _trackingTarget;
    private bool IsCloseEnough => TargetAngle <= _aimAffordance;
    private Vector3 AimVector => _aimReference.forward;
    private Vector3 AimRightNormal => _aimReference.right;
    private Vector3 AimUpNormal => _aimReference.up;
    private Vector3 TargetVector => (_trackingTarget.position - _aimReference.position).normalized;
    private float TargetAngle => Vector3.Angle(TargetVector, AimVector);

    private void OnEnable()
    {
        _detector.onAgentDetectionStatusChanged += OnAgentDetected;
    }

    private void OnDisable()
    {
        _detector.onAgentDetectionStatusChanged -= OnAgentDetected;
    }
    
    protected override void Start()
    {
        base.Start();
        StartCoroutine(LifeLoop());
    }
    
    private IEnumerator LifeLoop()
    {
        _lifeIndicator.SetBool(IsAlive);
        
        while (IsAlive)
        {
            if (HasTarget) yield return StartCoroutine(TrackingLoop());

            yield return null;
        }

        _lifeIndicator.SetBool(IsAlive);
    }

    private IEnumerator TrackingLoop()
    {
        float trackedTime = 0f;
        while (HasTarget)
        {
            AimAt(_trackingTarget);

            trackedTime = IsCloseEnough ? trackedTime + Time.deltaTime : 0f;

            if (_isLoaded && trackedTime >= _trackingFireDelay) Fire();
                
            yield return null;
        }
    }

    private IEnumerator ReloadSequence()
    {
        _isLoaded = false;
        yield return new WaitForSeconds(_reloadDelay);
        _isLoaded = true;
    }

    private void OnAgentDetected(AgentController agentController, bool isDetected)
    {
        // TODO: Add more advanced logic for picking targets

        if (isDetected)
        {
            if (!_trackingTarget)
            {
                _trackingTarget = agentController.transform;
            }
        }
        else
        {
            if (_trackingTarget == agentController.transform)
            {
                _trackingTarget = null;

                if (_detector.Agents.Count > 0)
                {
                    _trackingTarget = _detector.Agents[0].transform;
                }
            }
        }
    }

    private void Fire()
    {
        _cannon.OnStartFire(true);
        StartCoroutine(ReloadSequence());
    }

    private void AimAt(Transform target)
    {
        if (IsCloseEnough) return;
        
        // Calculate direction
        int panDirectionIndex = Vector3.Angle(TargetVector, AimRightNormal) < Vector3.Angle(AimVector, -AimRightNormal) ? 1 : -1;
        float panRotation = _panPivot.rotation.eulerAngles.y;
        panRotation += panDirectionIndex * _panSpeed * Time.deltaTime;
        int tiltDirectionIndex = Vector3.Angle(TargetVector, AimUpNormal) > Vector3.Angle(AimVector, -AimUpNormal) ? 1 : -1;
        float tiltRotation = _tiltPivot.rotation.eulerAngles.x;
        tiltRotation += tiltDirectionIndex * _tiltSpeed * Time.deltaTime;

        // Apply motion
        _panPivot.localRotation = Quaternion.Euler(0f, panRotation, 0f);
        _tiltPivot.localRotation = Quaternion.Euler(tiltRotation, 0f, 0f);
    }
    
    public void AssignDebugTarget(bool assign)
    {
        _trackingTarget = assign ? _debugTarget : null;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(FixedCannon))]
public class FixedCannonEditor : Editor
{
    private FixedCannon _target;

    private void Awake()
    {
        _target = (FixedCannon)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Assign Debug Target"))
        {
            _target.AssignDebugTarget(true);
        }

        if (GUILayout.Button("Unassign Debug Target"))
        {
            _target.AssignDebugTarget(false);
        }
    }
}
#endif