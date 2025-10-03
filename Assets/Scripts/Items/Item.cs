using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Item : MonoBehaviour, IPickup
{
    [SerializeField] protected string _itemName;

    private Transform _holdingTransform;

    protected Collider _collider;
    
    protected virtual void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    public virtual bool TryPickup(Transform holdingTransform, out IPickup pickup)
    {
        pickup = this;
        Pickup(holdingTransform);
        return true;
    }

    public virtual void Pickup(Transform newHoldingTransform)
    {
        _holdingTransform = newHoldingTransform;
        transform.parent = _holdingTransform;
        transform.localPosition = Vector3.zero;
        transform.rotation = _holdingTransform.rotation;
        _collider.enabled = false;
    }

    public virtual void Drop()
    {
        _holdingTransform = null;
        _collider.enabled = true;
    }

    public void Place(IReceiver receiver)
    {
        _holdingTransform = receiver.GetNewParent();
        transform.parent = _holdingTransform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        _collider.enabled = false;
    }

    public virtual void Interact() { }
    public GameObject GetGameObject() { return gameObject; }
    public string GetInteractionPrompt() { return _itemName; }
}
