using UnityEngine;

namespace _Project.Scripts.Data.Memory
{
    [CreateAssetMenu(fileName = "Memory Data", menuName = "Game/Memory Data", order = 0)]
    public class MemoryData : ScriptableObject
    {
        public string memoryId;
        public string sceneName;
        public string belongsToNpcId;
    }
}