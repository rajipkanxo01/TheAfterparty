using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Dialogue;
using _Project.Scripts.Application.Player;
using UnityEngine;

namespace _Project.Scripts.Presentation.Clues
{
    public class ContradictionObject : MonoBehaviour, IInteractable
    {
        [SerializeField] private string memoryId;
        [SerializeField] private string observationId;
        [SerializeField] private string lockedYarnNode;
        [SerializeField] private string unlockedYarnNode;
        
        private PlayerProfile _playerProfile;

        private void Start()
        {
            _playerProfile = ServiceLocater.GetService<PlayerProfile>();
            if (_playerProfile == null)
            {
                Debug.LogError("ContradictionObject: PlayerProfile not found.");
            }
        }
        
        public void Interact(GameObject interactor)
        {
            var completedMemory = _playerProfile.HasFragmentCompletedMemory(memoryId);
            DialogueEvents.RaiseDialogueStart(!completedMemory ? lockedYarnNode : unlockedYarnNode, DialogueType.PlayerMonologue);
        }

        public string GetInteractionPrompt()
        {
            return "Present Contradiction";
        }
    }
}