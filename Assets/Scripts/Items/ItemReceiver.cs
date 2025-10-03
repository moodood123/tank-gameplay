using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemReceiver : MonoBehaviour, IReceiver
{
    [SerializeField] private string _interactionPrompt;
    [SerializeField] private Transform _holdingTransform;

    protected IPickup _currentItem;
    public bool IsStoringItem => _currentItem != null;

    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }
    
    public bool TryPlaceItem(IPickup item)
    {
        if (!IsStoringItem && IsItemCompatible(item))
        {
            _currentItem = item;
            _currentItem.Place(this);
            return true;
        }
        return false;
    }

    public bool TryCollectItem(Transform newHoldingTransform, out IPickup item)
    {
        item = _currentItem;

        if (IsStoringItem)
        {
            _currentItem.TryPickup(newHoldingTransform, out item);
            _currentItem = null;
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual bool IsItemCompatible(IPickup item)
    {
        return true;
    }

    public Transform GetNewParent() { return _holdingTransform; }

    public string GetInteractionPrompt()
    {
        if (IsStoringItem) return _currentItem.GetInteractionPrompt();
        return _interactionPrompt;
    }
    public void Interact() { }
    public GameObject GetGameObject() { return gameObject; }
}
