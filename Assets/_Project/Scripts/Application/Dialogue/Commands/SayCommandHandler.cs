using System.Threading.Tasks;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Player;
using _Project.Scripts.Data.Npc;
using UnityEngine;

namespace _Project.Scripts.Application.Dialogue.Commands
{
    public class SayCommandHandler : IDialogueCommandHandler
    {
        private readonly DialogueController _dialogueController;
        private readonly PlayerProfile _playerProfile;
        private readonly NpcDatabase _npcDatabase;
        private readonly DialogueSpeakerResolver _speakerResolver;
        
        private TaskCompletionSource<bool> _waitForContinue;

        public string CommandName => "say";

        public SayCommandHandler()
        {
            _dialogueController = ServiceLocater.GetService<DialogueController>();
            _playerProfile = ServiceLocater.GetService<PlayerProfile>();
            _npcDatabase = ServiceLocater.GetService<NpcDatabase>();
            _speakerResolver = ServiceLocater.GetService<DialogueSpeakerResolver>();
            
            DialogueEvents.OnDialogueContinueRequested += HandleContinue;
        }

        public async Task ExecuteAsync(string[] parameters)
        {
            if (parameters == null || parameters.Length < 2)
            {
                Debug.LogWarning("SayCommandHandler: Invalid parameters for 'say'");
                return;
            }

            string speakerId = parameters[0];
            string line = parameters[1];

            var (displayName, portrait) = _speakerResolver.Resolve(speakerId, _playerProfile, _dialogueController.CurrentType);

            Debug.Log($"SayCommandHandler: Speaker '{speakerId}' says: {line}");
            DialogueEvents.RaiseDialogueLineStarted(_dialogueController, new DialogueLineEventArgs(displayName, line, portrait, speakerId));

            // Wait until the player continues
            
            
            _waitForContinue = new TaskCompletionSource<bool>();

            
            Debug.Log("SayCommandHandler: Waiting for player to continue...");
            await _waitForContinue.Task;

            DialogueEvents.RaiseDialogueContinued();
            Debug.Log("SayCommandHandler: Player requested to continue dialogue...");
        }
        
        private void HandleContinue()
        {
            if (_waitForContinue != null && !_waitForContinue.Task.IsCompleted)
            {
                _waitForContinue.SetResult(true);
            }
        }

    }
}