using PrimeTween;
using UnityEngine;
using UnityEngine.Events;

public class WorldspaceButton : MonoBehaviour, IInteractable
{
    [SerializeField] private string _interactionPrompt;

    [Header("Animation Settings")] 
    [SerializeField] private Transform _buttonTransform;
    [SerializeField] private TweenSettings<float> _pressSettings;
    
    [Header("Events")] 
    [SerializeField] private UnityEvent _onInteract;
    
    public string GetInteractionPrompt()
    {
        return _interactionPrompt;
    }

    public void Interact()
    {
        Tween.LocalPositionY(_buttonTransform, _pressSettings);
        _onInteract.Invoke();
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
