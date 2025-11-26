using _Project.Scripts.Application.Clue;
using UnityEngine;

namespace _Project.Scripts.Presentation.Clues
{
    public class ContradictionObject : MonoBehaviour, IInteractable
    {
        [SerializeField] private string memoryId;
        [SerializeField] private string observationId;
        

        public void Interact(GameObject interactor)
        {
            if (interactor.CompareTag("Player"))
            {
                ContradictionEvents.RaiseContradictionFound(memoryId, observationId);
            }
        }

        public string GetInteractionPrompt()
        {
            return "Present Contradiction";
        }
    }
}