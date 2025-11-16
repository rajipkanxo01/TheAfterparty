using System.Collections.Generic;
using _Project.Scripts.Data.Memory.Actions;
using UnityEngine;

namespace _Project.Scripts.Data.Memory.Fragments
{
    [CreateAssetMenu(fileName = "New Fragment", menuName = "Game/Memory/Fragment", order = 0)]
    public class FragmentData : ScriptableObject
    {
        [Header("Fragment Info")]
        public string fragmentId;
        public int orderInMemory;
        
        [Header("Actions")] // Each fragment will contain list of actions
        [Header("Memory Versions")]
        public List<ActionBaseData> realMemoryActions = new();
        public List<ActionBaseData> corruptedMemoryActions = new();
        
        [Header("Memory State")]
        [Tooltip("If true, the player has repaired this fragment and will see the real memory.")]
        public bool isRepaired;
        
        [Tooltip("If true, this fragment starts corrupted and shows the corrupted version until repaired.")]
        public bool isCorrupted;
        
        public bool HasCorruptedVersion => corruptedMemoryActions is { Count: > 0 };
    }
}