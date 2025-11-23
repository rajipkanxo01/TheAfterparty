using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Dialogue;
using UnityEngine;

namespace _Project.Scripts.Presentation.Npc
{
    public class NpcDialogueTrigger : MonoBehaviour, IInteractable
    {
        [SerializeField] private string npcId;
        [SerializeField] private string yarnRootNode;
        [SerializeField] private Transform headPoint;

        private DialogueController _dialogueControl;
        private GameStateService _gameStateService;

        private void Start()
        {
            _dialogueControl = ServiceLocater.GetService<DialogueController>();
            if (_dialogueControl == null)
            {
                Debug.LogError("NpcDialogueTrigger: DialogueController not found in scene.");
            }
            
            _gameStateService = ServiceLocater.GetService<GameStateService>();
        }

        public void Interact(GameObject interactor)
        {
            _dialogueControl.StartDialogue(yarnRootNode);
            _gameStateService.SetState(GameState.Dialogue);
        }

        public string GetInteractionPrompt()
        {
            return "Talk";
        }
        
        public Transform GetHeadPoint()
        {
            return headPoint != null ? headPoint : transform;
        }
    }
}