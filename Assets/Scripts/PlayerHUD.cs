using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _interactionPrompt;
    
    private PlayerController _pc;
    
    private void Awake()
    {
        _pc = GetComponentInParent<PlayerController>();
    }

    private void OnEnable()
    {
        _pc.onInteractableChanged += OnInteractableChanged;
    }

    private void OnDisable()
    {
        _pc.onInteractableChanged -= OnInteractableChanged;
    }

    private void OnInteractableChanged(IInteractable interactable)
    {
        if (interactable != null)
        {
            _interactionPrompt.text = interactable.GetInteractionPrompt();
            _interactionPrompt.gameObject.SetActive(true);
        }
        else
        {
            _interactionPrompt.gameObject.SetActive(false);
        }
    }
}
