using UnityEngine;
using UnityEngine.UI;

public class LevelLoaderButton : SceneLoaderButton
{
    [SerializeField] private Button _button;
    
    private void OnEnable()
    {
        LevelManager.OnLevelSelected += EnableButton;
        LevelManager.OnLevelDeselected += DisableButton;
    }

    private void OnDisable()
    {
        LevelManager.OnLevelSelected -= EnableButton;
        LevelManager.OnLevelDeselected -= DisableButton;
    }

    private void EnableButton()
    {
        _button.interactable = true;
    }

    private void DisableButton()
    {
        _button.interactable = false;
    }
}
