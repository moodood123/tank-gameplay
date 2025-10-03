using System.Collections;
using UnityEditor;
using UnityEngine;

public class MachineGun : Gun
{
    
    [SerializeField] private float _shotInterval = 0.1f;
    [SerializeField] private float _startDelay = 0.5f;
    
    [Header("Animation Settings")]
    [SerializeField] private GameObject _casingPrefab;
    [SerializeField] private Transform _casingSpawnPoint;
    [SerializeField] private Vector3 _casingImpulse = new Vector3();
    
    private Coroutine _fireLoop;

    private IEnumerator FireLoop()
    {
        yield return new WaitForSeconds(_startDelay);
        while (true)
        {
            Fire();
            //EjectCasing();
            yield return new WaitForSeconds(_shotInterval);
            yield return null;
        }
    }

    private void EjectCasing()
    {
        GameObject go = Instantiate(_casingPrefab, _casingSpawnPoint.position, _casingSpawnPoint.rotation);
        if (go.TryGetComponent(out Rigidbody rb))
        {
            rb.AddRelativeForce(_casingImpulse, ForceMode.Impulse);
        }
    }

    public override void OnStartFire(bool debug = false)
    {
        if (_fireLoop != null) return;

        base.OnStartFire(debug);
        _fireLoop = StartCoroutine(FireLoop());
    }

    public override void OnStopFire()
    {
        base.OnStopFire();
    
        if (_fireLoop != null)
        {
            StopCoroutine(_fireLoop);
            _fireLoop = null; // ← important!
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MachineGun))]
public class MachineGunEditor : Editor
{
    private MachineGun _target;

    private void Awake()
    {
        _target = (MachineGun)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Start")) _target.OnStartFire();
        if (GUILayout.Button("Stop")) _target.OnStopFire();
    }
}
#endif