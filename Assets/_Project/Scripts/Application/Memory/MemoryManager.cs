using System;
using System.Collections.Generic;
using _Project.Scripts.Data;

namespace _Project.Scripts.Application.Memory
{
    public class MemoryManager
    {
        public event Action MemoryTransitionStarted;
        public event Action<string> MemoryTransitionCompleted;
        
        private string targetScene;
        
        private readonly List<NpcData> unlockedNpcsMemory = new List<NpcData>();
        
        public MemoryManager()
        {
        }

        public void VisitMemory(NpcData memory)
        {
            if (!memory.isUnlocked)
            {
                memory.isUnlocked = true;
                unlockedNpcsMemory.Add(memory);
            }

            targetScene = memory.memorySceneName;
            MemoryTransitionStarted?.Invoke();
        }
        
        public void RevisitMemory(string sceneName)
        {
            targetScene = sceneName;
            MemoryTransitionStarted?.Invoke();
        }

        public void OnVisualsDone()
        {
            MemoryTransitionCompleted?.Invoke(targetScene);
        }
        
        public IEnumerable<NpcData> GetUnlockedMemories() => unlockedNpcsMemory;
    }
}
