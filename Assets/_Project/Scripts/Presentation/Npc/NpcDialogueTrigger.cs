using _Project.Scripts.Application;
using _Project.Scripts.Application.Dialogue;
using _Project.Scripts.Presentation.Dialogue;
using UnityEngine;

namespace _Project.Scripts.Presentation.Npc
{
    public class NpcDialogueTrigger : MonoBehaviour, IInteractable
    {
        [SerializeField] private string npcId;
        [SerializeField] private Transform headPoint;

        private DialogueService _dialogueService;
        private DialogueView _dialogueView;

        private void Start()
        {
            _dialogueService = ServiceLocater.GetService<DialogueService>();
            _dialogueView = ServiceLocater.GetService<DialogueView>();
        }

        public void Interact(GameObject interactor)
        {
            if (_dialogueService == null || _dialogueView == null)
            {
                Debug.LogWarning($"[NpcDialogueTrigger] Dialogue system not found for NPC {npcId}.");
                return;
            }

            _dialogueView.SetTarget(headPoint != null ? headPoint : transform);
            _dialogueService.StartDialogue(npcId);
        }

        public string GetInteractionPrompt()
        {
            return "Talk";
        }
    }
}