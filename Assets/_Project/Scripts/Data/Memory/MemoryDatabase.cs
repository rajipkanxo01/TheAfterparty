using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Project.Scripts.Data.Memory
{
    [CreateAssetMenu(fileName = "Memory Database", menuName = "Game/Database/Memory Database", order = 0)]
    public class MemoryDatabase : ScriptableObject
    {
        [SerializeField] private List<MemoryData> memories = new();
        
        public List<MemoryData> GetAllMemories()
        {
            return memories;
        }
        
        public int GetMemoryCount()
        {
            return memories.Count;
        }
        
        

        public MemoryData GetById(string memoryId)
        {
            var memoryData = memories.First(data => data.memoryId.Equals(memoryId, StringComparison.InvariantCultureIgnoreCase));
            return memoryData;
        }
    }
}