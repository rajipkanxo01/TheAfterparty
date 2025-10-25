using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Data.Npc
{
    [CreateAssetMenu(fileName = "New NPC Data", menuName = "Game/NPC", order = 0)]
    public class NpcData : ScriptableObject
    {
        [Header("Identity")]
        public string npcId;
        public string npcName;
        public Sprite portrait;
        public string yarnStartNode;
        
        [Header("Memory")]
        public string memorySceneName;
        public bool isUnlocked;
    }
}