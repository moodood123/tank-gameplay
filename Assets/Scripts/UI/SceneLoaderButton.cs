using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderButton : MonoBehaviour
{
    [SerializeField] protected string _sceneToLoad;

    public void LoadScene()
    {
        if (SceneTransitionHandler.Instance) SceneTransitionHandler.Instance.ChangeScene(_sceneToLoad);
        else SceneManager.LoadScene(_sceneToLoad);
    }
}
