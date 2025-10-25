using UnityEngine;

namespace _Project.Scripts.Presentation
{
    public interface IInteractable
    {
        void Interact(GameObject interactor);
        string GetInteractionPrompt();
    }
}