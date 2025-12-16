using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Dialogue;
using UnityEngine;

namespace _Project.Scripts.Presentation.Npc
{
    public class NpcDialogueTrigger : MonoBehaviour, IInteractable
    {
        [Header("Dialogue Settings")]
        [SerializeField] private string npcId;
        [SerializeField] private string yarnRootNode;
        [SerializeField] private Transform headPoint;
        
        [Header("Trigger Settings")]
        [SerializeField] private TriggerType triggerType = TriggerType.OnInteract;
        
        [Tooltip("Tag that must match for trigger to activate (leave empty for any)")]
        [SerializeField] private string requiredTag = "Player";
        
        [Tooltip("If true, this trigger will only fire once and then disable itself")]
        [SerializeField] private bool triggerOnce;

        private DialogueController _dialogueControl;
        private GameStateService _gameStateService;
        private bool _hasTriggered;

        public enum TriggerType
        {
            OnInteract,
            OnTriggerEnter,
            Manual
        }

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
            if (triggerType != TriggerType.OnInteract) return;
            
            StartDialogue();
        }

        public string GetInteractionPrompt()
        {
            return "Talk";
        }
        
        public Transform GetHeadPoint()
        {
            return headPoint != null ? headPoint : transform;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (triggerType != TriggerType.OnTriggerEnter) return;
            
            if (ShouldTrigger(other.tag))
            {
                StartDialogue();
            }
        }
        
        private bool ShouldTrigger(string objectTag)
        {
            if (_hasTriggered && triggerOnce)
            {
                return false;
            }
            
            if (!string.IsNullOrEmpty(requiredTag) && objectTag != requiredTag)
            {
                return false;
            }
            
            return true;
        }
        
        private void StartDialogue()
        {
            if (_hasTriggered && triggerOnce)
            {
                return;
            }
            
            _dialogueControl.StartDialogue(yarnRootNode);
            _gameStateService.SetState(GameState.Dialogue);
            
            if (triggerOnce)
            {
                _hasTriggered = true;
            }
        }
        
        public void TriggerDialogue()
        {
            StartDialogue();
        }
    }
}