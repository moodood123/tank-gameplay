using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    private void Start()
    {
        LevelData data = LevelManager.CurrentLevelData;
        if (!data) return;

        if (!NetworkManager.Singleton) SceneManager.LoadSceneAsync(data.LevelSceneName, LoadSceneMode.Additive);
        else NetworkManager.Singleton.SceneManager.LoadScene(data.LevelSceneName, LoadSceneMode.Additive);
    }
}
