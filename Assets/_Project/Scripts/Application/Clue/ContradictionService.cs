using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Dialogue;
using _Project.Scripts.Application.Player;
using _Project.Scripts.Data;

namespace _Project.Scripts.Application.Clue
{
    public class ContradictionService
    {
        private ContradictionDialogueMap _dialogueMap;
        private readonly DialogueController _dialogueController;
        private readonly PlayerProfile _playerProfile;

        public ContradictionService(PlayerProfile playerProfile, ContradictionDialogueMap contradictionDialogueMap)
        {
            _playerProfile = playerProfile;
            _dialogueMap = contradictionDialogueMap;

            _dialogueController = ServiceLocater.GetService<DialogueController>();
            
            Initialize();
        }

        private void Initialize()
        {
            ContradictionEvents.OnContradictionFound += HandleContradictionFound;
        }
        
        private void HandleContradictionFound(string observationId)
        {
            string nodeName = _dialogueMap.GetNode(observationId);
            
            _playerProfile.SetContradicted(observationId);
            _dialogueController.StartDialogue(nodeName, DialogueType.PlayerMonologue);
            
            ToastNotification.Show("New Contradiction Recorded! Check your Journal.");
            
            // DialogueEvents.RaiseDialogueStart(nodeName, DialogueType.PlayerMonologue);
        }
        
        public void Dispose()
        {
            ContradictionEvents.OnContradictionFound -= HandleContradictionFound;
        }
    }
}