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
        
        
        // when transitioning out of memory
        public static event Action OnMemoryTransitionEnd;
        
        
        // when all fragments completed
        public static event Action OnAllFragmentsCompleted;
        
        
        // when repairing a fragment
        public static event Action<string> OnRepairFragment;
        
        
       
        
        public static void RaiseMemoryTransitionStart()
        {
            
            OnMemoryTransitionStart?.Invoke();
        }
        
        public static void RaiseMemoryTransitionEnd()
        {
            OnMemoryTransitionEnd?.Invoke();
        }
        
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