using UnityEngine;

public interface IReceiver : IInteractable
{
    public bool TryPlaceItem(IPickup item);
    public bool TryCollectItem(Transform potentialHoldingTransform, out IPickup item);
    public bool IsItemCompatible(IPickup item);
    public Transform GetNewParent();
}
