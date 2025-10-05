using System.Collections;
using PrimeTween;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionHandler : MonoBehaviour
{
    [SerializeField] private Image _fadePanel;
    [SerializeField] private TweenSettings<float> _fadeInSettings;
    [SerializeField] private TweenSettings<float> _fadeOutSettings;
    [SerializeField] private bool _fadeInOnStart = true;

    private bool _isSceneLoadInProgress = false;
    
    public static SceneTransitionHandler Instance;
    
    private void Awake()
    {
        if (!Instance) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    private void Start()
    {
        if (_fadeInOnStart)
        {
            Color color = _fadePanel.color;
            color.a = _fadeInSettings.startValue;
            _fadePanel.color = color;
            StartCoroutine(FadeSequence(_fadeInSettings));
        }
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(FadeSequence(_fadeOutSettings, sceneName));
    }

    private IEnumerator FadeSequence(TweenSettings<float> settings, string sceneToLoad = null)
    {
        _isSceneLoadInProgress = true;
        yield return Tween.Alpha(_fadePanel, settings).ToYieldInstruction();
        _isSceneLoadInProgress = false;
        if (sceneToLoad != null) SceneManager.LoadScene(sceneToLoad);
    }
}
