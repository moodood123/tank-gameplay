using UnityEngine;

public interface IInteractable
{
    public string GetInteractionPrompt();
    public void Interact();
    public GameObject GetGameObject();
}
