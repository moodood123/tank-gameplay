using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderButton : MonoBehaviour
{
    [SerializeField] private string _sceneToLoad;

    public void LoadScene()
    {
        if (SceneTransitionHandler.Instance) SceneTransitionHandler.Instance.ChangeScene(_sceneToLoad);
        else SceneManager.LoadScene(_sceneToLoad);
    }
}
