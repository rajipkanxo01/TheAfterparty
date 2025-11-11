using System.Collections.Generic;
using _Project.Scripts.Data.Memory.Fragments;
using UnityEngine;

namespace _Project.Scripts.Data.Memory
{
    [CreateAssetMenu(fileName = "Memory Data", menuName = "Game/Memory/Memory Data", order = 0)]
    public class MemoryData : ScriptableObject
    {
        [Header("Memory Info")]
        public string memoryId;
        public string sceneName;
        public string belongsToNpcId;
        
        [Header("Fragments")] // Each memory will contain list of fragments
        public List<FragmentData> fragments;
    }
}