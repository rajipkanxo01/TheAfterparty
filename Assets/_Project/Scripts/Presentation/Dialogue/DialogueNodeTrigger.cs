using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Dialogue;
using UnityEngine;

namespace _Project.Scripts.Presentation.Dialogue
{
    public class DialogueNodeTrigger : MonoBehaviour, IInteractable
    {
        [Header("Dialogue Settings")]
        [SerializeField] private string yarnNodeName;
        
        [Tooltip("The type of dialogue (NPC conversation or player monologue)")]
        [SerializeField] private DialogueType dialogueType = DialogueType.NpcConversation;
        
        [Header("Interaction Settings")]
        [Tooltip("The prompt text shown to the player")]
        [SerializeField] private string interactionPrompt = "Talk";
        
        private DialogueController _dialogueController;
        private GameStateService _gameStateService;

        private void Start()
        {
            _dialogueController = ServiceLocater.GetService<DialogueController>();
            _gameStateService = ServiceLocater.GetService<GameStateService>();
            
            if (_dialogueController == null)
            {
                Debug.LogError($"DialogueNodeTrigger on {gameObject.name}: DialogueController not found in scene.");
            }

            if (string.IsNullOrEmpty(yarnNodeName))
            {
                Debug.LogWarning($"DialogueNodeTrigger on {gameObject.name}: Yarn node name is not set.");
            }
        }

        public void Interact(GameObject interactor)
        {
            if (!interactor.CompareTag("Player")) return;

            if (_dialogueController == null)
            {
                Debug.LogError($"DialogueNodeTrigger on {gameObject.name}: DialogueController is not available.");
                return;
            }

            if (string.IsNullOrEmpty(yarnNodeName))
            {
                Debug.LogError($"DialogueNodeTrigger on {gameObject.name}: Cannot start dialogue - yarn node name is empty.");
                return;
            }

            _dialogueController.StartDialogue(yarnNodeName, dialogueType);
            _gameStateService?.SetState(GameState.Dialogue);
        }

        public string GetInteractionPrompt()
        {
            return interactionPrompt;
        }
    }
}

