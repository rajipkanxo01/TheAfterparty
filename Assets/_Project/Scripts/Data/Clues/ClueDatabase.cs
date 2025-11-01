using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Data.Clues
{
    [CreateAssetMenu(fileName = "Clue Database", menuName = "Game/ClueDatabase", order = 0)]
    public class ClueDatabase : ScriptableObject
    {
        [SerializeField] private List<ClueData> clues;
        
        public ClueData GetClueById(string clueId)
        {
            return clues.Find(clue => clue.clueId == clueId);
        }
        
        public List<ClueData> GetAllClues()
        {
            return clues;
        }
    }
}