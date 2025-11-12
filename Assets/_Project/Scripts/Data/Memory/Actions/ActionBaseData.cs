using UnityEngine;

namespace _Project.Scripts.Data.Memory.Actions
{
    public enum ActionPlayMode
    {
        Sequential,   // Wait for previous to finish
        Parallel,     // Run together with previous
        DelayedFromFragmentStart        // Start after delay from previous
    }
    
    // Base class for different types of action data related to memory fragments
    public abstract class ActionBaseData : ScriptableObject
    {
        [Tooltip("Order of this action within the fragment.")]
        public int order;
        public ActionPlayMode playMode  = ActionPlayMode.Sequential;
        public float startOffset = 0f;
        public float startDelaySeconds;
    }
}