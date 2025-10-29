using _Project.Scripts.Application;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Dialogue;
using UnityEngine;

namespace _Project.Scripts.Presentation.Npc
{
    public class NpcDialogueTrigger : MonoBehaviour, IInteractable
    {
        [SerializeField] private string npcId;
        [SerializeField] private Transform headPoint;

        private DialogueController _dialogueController;

        private void Start()
        {
            _dialogueController = ServiceLocater.GetService<DialogueController>();
            if (_dialogueController == null)
            {
                Debug.LogError("NpcDialogueTrigger: DialogueController not found in scene.");
            }
        }

        public void Interact(GameObject interactor)
        {
            Debug.Log($"NpcDialogueTrigger: Interact with NPC {npcId}");
            _dialogueController.StartDialogueWithNpc(npcId);
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