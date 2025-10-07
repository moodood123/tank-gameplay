using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShellItem : Item
{
    [SerializeField] private GameObject _liveMesh;
    [SerializeField] private GameObject _physicalCollider;
    [field: SerializeField] public GameObject ProjectilePrefab { get; private set; }
    [SerializeField] private Collider _interactTrigger;
    [SerializeField] private float _ejectionForce;

    public bool IsLive { get; private set; } = true;

    private Rigidbody _rb;
    
    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
    }
    
    public void Spend()
    {
        if (!IsLive) return;

        IsLive = false;
        _liveMesh.SetActive(false);
        _physicalCollider.SetActive(true);
        transform.parent = null;

        if (TryGetComponent<Rigidbody>(out var rb) == false)
            rb = gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.AddForce(-transform.forward * _ejectionForce, ForceMode.Impulse);

        // Forcefully clear held item if held
        Drop();
    }
}
