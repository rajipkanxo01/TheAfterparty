using UnityEngine;

namespace _Project.Scripts.Data
{
    [CreateAssetMenu(fileName = "New NPC Data", menuName = "Data/NPC Data", order = 0)]
    public class NpcData : ScriptableObject
    {
        public string npcName;
        public string memorySceneName;
        public bool isUnlocked;
    }
}