using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Data/LevelData")]
public class LevelData : ScriptableObject
{
    [field: SerializeField] public string LevelTitle { get; private set; }
    [field: SerializeField] public string LevelSceneName { get; private set; }
}