using UnityEngine;

public interface IPickup : IInteractable
{
    public bool TryPickup(Transform holdingTransform, out IPickup pickup);

    public void Drop();

    public void Place(IReceiver receiver);
}
