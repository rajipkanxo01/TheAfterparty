using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Dialogue;
using _Project.Scripts.Application.Player;
using _Project.Scripts.Data;

namespace _Project.Scripts.Application.Clue
{
    public class NotesService
    {
        private ContradictionDialogueMap _dialogueMap;
        private readonly DialogueController _dialogueController;
        private readonly PlayerProfile _playerProfile;

        public NotesService(PlayerProfile playerProfile, ContradictionDialogueMap contradictionDialogueMap)
        {
            _playerProfile = playerProfile;
            _dialogueMap = contradictionDialogueMap;

            _dialogueController = ServiceLocater.GetService<DialogueController>();
            
            Initialize();
        }

        private void Initialize()
        {
            NotesEvent.OnNotesFound += HandleNotesFound;
        }
        
        private void HandleNotesFound(ObservationState state, string memoryId, string observationId)
        {
            string nodeName = _dialogueMap.GetNode(observationId);
            
            _playerProfile.SetObservationState(memoryId, observationId, state);
            _dialogueController.StartDialogue(nodeName, DialogueType.PlayerMonologue);
            
            // Show appropriate toast message based on state
            string toastMessage = state == ObservationState.Verified 
                ? "Observation Verified! Check your Notes." 
                : "New Contradiction Recorded! Check your Notes.";
            
            ToastNotification.Show(toastMessage);
            
            // Trigger UI update in journal
            Events.UIEvents.RaiseJournalNotesChanged();
            
            // DialogueEvents.RaiseDialogueStart(nodeName, DialogueType.PlayerMonologue);
        }
        
        public void Dispose()
        {
            NotesEvent.OnNotesFound -= HandleNotesFound;
        }
    }
}