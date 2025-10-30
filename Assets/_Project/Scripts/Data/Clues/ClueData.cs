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
        public GameObject cluePrefab;
        public bool isImportantClue;
        
        [TextArea] public string description;
        public string dialogueNode;
        
        [TextArea] public List<string> hints; 
        public string sniffDialogueNode;
        
    }
}