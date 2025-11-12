using System;
using UnityEngine;

namespace _Project.Scripts.Application.Memory
{
    public static class MemoryEvents
    {
        public static event Action<string> OnMemoryUnlocked;
        public static event Action<string> OnVisitMemory;
        public static event Action OnMemoryTransitionStart;
        public static event Action OnMemoryTransitionEnd;
        public static event Action OnAllFragmentsCompleted;
        public static event Action<string> OnRepairFragment;
        
        public static void RaiseMemoryUnlocked(string memoryId)
        {
            OnMemoryUnlocked?.Invoke(memoryId);
        }
        
        public static void RaiseVisitMemory(string memoryId)
        {
            OnVisitMemory?.Invoke(memoryId);
        }
        
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