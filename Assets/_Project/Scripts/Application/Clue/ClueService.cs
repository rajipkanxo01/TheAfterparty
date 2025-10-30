using _Project.Scripts.Application.Core;

namespace _Project.Scripts.Application.Clue
{
    public class ClueService
    {
        private readonly ClueManager _clueManager;
        private readonly GameStateService _gameStateService;
        // private readonly DialogueController _dialogueController;

        public ClueService(ClueManager clueManager, GameStateService gameStateService)
        {
            _clueManager = clueManager;
            _gameStateService = gameStateService;
        }

        public void Examine(string clueId)
        {
            if (!_gameStateService.IsState(GameState.Normal)) return;

            var clueData = _clueManager.GetClueById(clueId);
            if (clueData == null)
            {
                UnityEngine.Debug.LogWarning($"ClueService: Clue with ID {clueId} not found.");
            }
            
            ClueEvents.RaiseExamined(clueData.dialogueNode);
        }

        public void Sniff(string clueId)
        {
            // todo: show hints or additional info about the clue
        }
    }
}