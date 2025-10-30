using _Project.Scripts.Data.Clues;

namespace _Project.Scripts.Application.Clue
{
    public class ClueManager
    {
        private readonly ClueDatabase _clueDatabase;
        public ClueManager(ClueDatabase clueDatabase)
        {
            _clueDatabase = clueDatabase;
        }
        
        public ClueData GetClueById(string clueId)
        {
            return _clueDatabase.GetClueById(clueId);
        }
        
        public void CompleteClue(string clueId)
        {
            var clue = _clueDatabase.GetClueById(clueId);
            if (clue == null)
            {
                UnityEngine.Debug.LogWarning($"ClueManager: Clue '{clueId}' not found in database.");
                return;
            }

            ClueEvents.RaiseDiscovered(clue);
        }
    }
}