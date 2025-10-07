using UnityEngine;

public class CannonLoader : ItemReceiver
{
    public ShellItem CurrentShell => _currentItem as ShellItem;

    public bool IsLoaded => CurrentShell != null;
    
    public void EjectSpentShell()
    {
        if (CurrentShell) CurrentShell.Spend();
        _currentItem = null;
    }
    
    public override bool IsItemCompatible(IPickup item)
    {
        return item is ShellItem;
    }
}
