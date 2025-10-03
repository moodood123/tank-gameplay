using UnityEngine;

public class ShellHolder : ItemReceiver
{
    [SerializeField] private ShellItem _initialShell;

    private void Start()
    {
        if (_initialShell != null)
        {
            IPickup pickup = _initialShell.GetComponent<IPickup>();

            if (IsItemCompatible(pickup))
            {
                bool placed = TryPlaceItem(pickup);

                if (!placed)
                {
                    Debug.LogWarning($"ShellHolder failed to place initial shell: {pickup.GetGameObject().name}");
                }
            }
            else
            {
                Debug.LogWarning($"Initial shell is not compatible: {_initialShell.name}");
            }
        }
    }
    
    public override bool IsItemCompatible(IPickup item)
    {
        return item is ShellItem shell && shell.IsLive;
    }
}