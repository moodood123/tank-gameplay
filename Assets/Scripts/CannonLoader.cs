using UnityEngine;

public class CannonLoader : ItemReceiver
{
    private ShellItem _currentShell => _currentItem as ShellItem;

    public bool IsLoaded => _currentShell != null;
    
    public void EjectSpentShell()
    {
        if (_currentShell) _currentShell.Spend();
        _currentItem = null;
    }
    
    public override bool IsItemCompatible(IPickup item)
    {
        return item is ShellItem;
    }
}
