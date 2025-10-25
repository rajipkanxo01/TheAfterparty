using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Data.Npc
{
    [CreateAssetMenu(fileName = "New NPC Data", menuName = "Data/NPC Data", order = 0)]
    public class NpcData : ScriptableObject
    {
        public string npcId;
        public string npcName;
        public string memorySceneName;
        public bool isUnlocked;
        public List<DialogueLine> dialogueLines;
    }
}