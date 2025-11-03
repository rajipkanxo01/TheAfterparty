using _Project.Scripts.Application.Clue;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Data.Clues;

namespace _Project.Scripts.Application.Dialogue
{
    public class ClueDialogueHandler
    {
        private readonly DialogueController _dialogueController;
        private ClueData _currentClue;
        private bool _isClueDialogue;
        private ClueManager _clueManager;

        public ClueDialogueHandler(DialogueController dialogueController)
        {
            _dialogueController = dialogueController;
            
            ClueEvents.OnClueExamined += HandleClueExamined;
            _clueManager = ServiceLocater.GetService<ClueManager>();
        }
        
        public void Disable()
        {
            ClueEvents.OnClueExamined -= HandleClueExamined;
        }
        
        private void HandleClueExamined(ClueData clue)
        {
            _isClueDialogue = true;
            _currentClue = clue;
            _dialogueController.StartDialogue(clue.dialogueNode, DialogueType.PlayerMonologue);
        }

        public void HandleDialogueEnd()
        {
            if (!_isClueDialogue || _clueManager == null)
            {
                return;
            }

            _clueManager.CompleteClue(_currentClue.clueId);
            _isClueDialogue = false;
            _currentClue = null;
        }
    }
}