using System;
using UnityEngine;

namespace _Project.Scripts.Application.Memory.Events
{
    public static class MemoryEvents
    {
        // when unlocking a new memory
        public static event Action<string> OnMemoryUnlocked;
        public static void RaiseMemoryUnlocked(string memoryId) => OnMemoryUnlocked?.Invoke(memoryId);
        
        
        // when visiting a memory
        public static event Action<string> OnVisitMemory;
        public static void RaiseVisitMemory(string memoryId)
        {
            OnVisitMemory?.Invoke(memoryId);
        }
        
        // when transitioning into memory
        public static event Action OnMemoryTransitionStart;
        public static void RaiseMemoryTransitionStart()
        {
            Debug.Log($"Raising OnMemoryTransitionStart event.");
            OnMemoryTransitionStart?.Invoke();
        }
        
        // when transitioning out of memory
        public static event Action OnMemoryTransitionEnd;
        public static void RaiseMemoryTransitionEnd()
        {
            OnMemoryTransitionEnd?.Invoke();
        }
        
        // when mogens learns chest code
        public static event Action<string> OnChestCodeLearned;
        public static void RaiseChestCodeLearned(string code) => OnChestCodeLearned?.Invoke(code);
        
        
        // when all fragments completed
        public static event Action OnAllFragmentsCompleted;
        
        
        // when repairing a fragment
        public static event Action<string> OnRepairFragment;
        
        
        public static void RaiseAllFragmentsCompleted()
        {
            OnAllFragmentsCompleted?.Invoke();
        }

        public static void RaiseRepairFragment(string fragmentId)
        {
            Debug.Log($"MemoryEvents: Raised repair fragment event for {fragmentId}");
            OnRepairFragment?.Invoke(fragmentId);
        }
    }
}