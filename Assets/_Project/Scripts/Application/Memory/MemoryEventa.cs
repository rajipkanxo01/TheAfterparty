using System;
using UnityEngine;

namespace _Project.Scripts.Application.Memory
{
    public static class MemoryEvents
    {
        public static event Action<string> OnMemoryUnlocked;
        
        public static void RaiseMemoryUnlocked(string memoryId)
        {
            OnMemoryUnlocked?.Invoke(memoryId);
        }
    }
}