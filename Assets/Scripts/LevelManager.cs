using UnityEngine;

public static class LevelManager
{
    public static LevelData CurrentLevelData { get; private set; }

    public delegate void OnLevelSelectionChanged();
    public static OnLevelSelectionChanged OnLevelSelected;
    public static OnLevelSelectionChanged OnLevelDeselected;

    public static void SetCurrentLevelData(LevelData levelData)
    {
        CurrentLevelData = levelData;
        OnLevelSelected?.Invoke();
    }
    
    public static void VoidCurrentLevelData()
    {
        CurrentLevelData = null;
        OnLevelDeselected?.Invoke();
    }
}
