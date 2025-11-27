using System;
using _Project.Scripts.Application.Clue;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Dialogue;
using _Project.Scripts.Application.Player;
using _Project.Scripts.Data.Clues;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Presentation.Player
{
    public class SniffInputHandler : MonoBehaviour
    {
        [SerializeField] private SniffConfig sniffConfig;
        [SerializeField] private bool showDebugRadius = true;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private string sniffDialogueNode;
        
        private ClueService _clueService;
        private DialogueController _dialogueController;
        private PlayerProfile _playerProfile;
        
        private void Start()
        {
            _clueService = ServiceLocater.GetService<ClueService>();
            _dialogueController = ServiceLocater.GetService<DialogueController>();
            _playerProfile = ServiceLocater.GetService<PlayerProfile>();
        }

        private void OnEnable()
        {
            ClueEvents.OnHintFound += HandleHintFound;
        }
        
        private void OnDisable()
        {
            ClueEvents.OnHintFound -= HandleHintFound;
        }

        public void OnSniffPerformed(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            var completedMemory = _playerProfile.HasFragmentCompletedMemory("badgeLost");
            if (completedMemory && !string.IsNullOrEmpty(sniffDialogueNode))
            {
                _dialogueController.StartDialogue(sniffDialogueNode);
            }
            else
            {
                ToastNotification.Show("Nothing to discover right now.");
            }
        }
        
        private void HandleHintFound(ClueData clueData)
        {
            // Additional handling if needed when a hint is found
            Debug.Log($"Hint found with dialogue node: {clueData.clueId}");
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!showDebugRadius || _clueService == null) return;

            Gizmos.color = Color.gold;
            Gizmos.DrawWireSphere(playerTransform.position, sniffConfig.SniffRadius);
        }
#endif
    }
}