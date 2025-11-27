using System;
using System.Collections.Generic;
using _Project.Scripts.Data.Memory.Fragments;
using UnityEngine;

namespace _Project.Scripts.Data.Memory
{
    [Serializable]
    public class MemoryObservation
    {
        public string observationId;
        [TextArea(2, 5)]
        public string observationText;
    }
    
    [CreateAssetMenu(fileName = "Memory Data", menuName = "Game/Memory/Memory Data", order = 0)]
    public class MemoryData : ScriptableObject
    {
        [Header("Memory Info")]
        public string memoryId;
        public string sceneName;
        public string belongsToNpcId;
        public string memoryTitle;

        public List<MemoryObservation> memoryObservations;

        [Header("Fragments")] // Each memory will contain list of fragments
        public List<FragmentData> fragments;
    }
}