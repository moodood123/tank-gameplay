using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    private void Start()
    {
        LevelData data = LevelManager.CurrentLevelData;
        if (data) SceneManager.LoadSceneAsync(data.LevelSceneName, LoadSceneMode.Additive);
    }
}
