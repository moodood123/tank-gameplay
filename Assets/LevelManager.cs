using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public LevelData CurrentLevel { get; private set; }
    
    public static LevelManager Instance;
    
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SetCurrentLevel(LevelData level)
    {
        CurrentLevel = level;
    }
}