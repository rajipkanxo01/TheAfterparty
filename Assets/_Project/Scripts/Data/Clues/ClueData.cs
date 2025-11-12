using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Data.Clues
{
    [CreateAssetMenu(fileName = "New Clue Data", menuName = "Game/Clue", order = 0)]
    public class ClueData : ScriptableObject
    {
        public string clueId;
        public string clueName;
        public Sprite clueIcon;
        
        [TextArea] public string description;
        [TextArea] public List<string> detectiveNotes;
        
        public string dialogueNode;
        public string sniffDialogueNode;
        
        [Header("Optional Link")]
        [Tooltip("If set, this clue can repair a specific memory fragment.")]
        public string repairsFragmentId;
        
    }
}