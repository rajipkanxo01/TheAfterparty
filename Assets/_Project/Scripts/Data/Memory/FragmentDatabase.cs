using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Data.Memory.Fragments;
using UnityEngine;

namespace _Project.Scripts.Data.Memory
{
    [CreateAssetMenu(fileName = "Fragment Database", menuName = "Game/Database/Fragment Database")]
    public class FragmentDatabase : ScriptableObject
    {
        [SerializeField] private List<FragmentData> fragments = new();
        
        public FragmentData GetById(string fragmentId)
        {
            if (string.IsNullOrWhiteSpace(fragmentId))
            {
                return null;
            }

            return fragments.FirstOrDefault(f =>
                f.fragmentId.Equals(fragmentId, StringComparison.InvariantCultureIgnoreCase));
        }

        public IReadOnlyList<FragmentData> GetAll()
        {
            return fragments;
        }
    }
}