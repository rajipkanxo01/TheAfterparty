using UnityEngine;

namespace _Project.Scripts.Data.Memory.Actions
{
    // Base class for different types of action data related to memory fragments
    public abstract class ActionBaseData : ScriptableObject
    {
        [Tooltip("Order of this action within the fragment.")]
        public int order;
    }
}