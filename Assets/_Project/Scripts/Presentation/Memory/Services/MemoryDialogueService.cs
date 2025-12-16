using System;
using System.Threading.Tasks;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Dialogue;
using _Project.Scripts.Application.Memory.Services;
using UnityEngine;

namespace _Project.Scripts.Presentation.Memory.Services
{
    public class MemoryDialogueService : MonoBehaviour, IMemoryDialogueService
    {
        private DialogueController _dialogueController;
        private TaskCompletionSource<bool> _dialogueFinished = new();
        private GameStateService _gameStateService;


        private void Awake()
        {
            ServiceLocater.RegisterService<IMemoryDialogueService>(this);
        }
        
        private void Start()
        {
            _dialogueController = ServiceLocater.GetService<DialogueController>();
            
            ServiceLocater.PrintAllServices();
            
            _gameStateService = ServiceLocater.GetService<GameStateService>();
            
            if (_dialogueController == null)
            {
                Debug.LogError("MemoryDialogueService: DialogueController service not found!");
                return;
            }
            
            DialogueEvents.OnDialogueEnded += HandleDialogueEnded;
        }

        private void OnDestroy()
        {
            DialogueEvents.OnDialogueEnded -= HandleDialogueEnded;
        }

        public async Task PlayNodeAsync(string nodeName)
        {
            if (string.IsNullOrEmpty(nodeName))
            {
                Debug.LogWarning("MemoryDialogueService: Empty node name.");
                return;
            }

            if (_dialogueController == null)
            {
                Debug.LogError("MemoryDialogueService: DialogueRunner missing!");
                return;
            }


            _dialogueFinished = new TaskCompletionSource<bool>();
            
            Debug.Log("MemoryDialogueService: Playing node: " + nodeName);
            _dialogueController.StartDialogue(nodeName);
            _gameStateService.SetState(GameState.Cutscene);

            await _dialogueFinished.Task;

        }
        
        private void HandleDialogueEnded()
        {
            _dialogueFinished?.TrySetResult(true);
        }
    }
}