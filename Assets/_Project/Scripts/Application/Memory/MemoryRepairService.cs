using System;
using System.Linq;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Memory.Events;
using _Project.Scripts.Application.Player;
using _Project.Scripts.Data.Memory;
using UnityEngine;

namespace _Project.Scripts.Application.Memory
{
    public class MemoryRepairService
    {
        private readonly MemoryDatabase _memoryDatabase;
        private readonly PlayerProfile _playerProfile;

        public MemoryRepairService(MemoryDatabase memoryDatabase)
        {
            _memoryDatabase = memoryDatabase;
            _playerProfile = ServiceLocater.GetService<PlayerProfile>();
            
            MemoryEvents.OnRepairFragment += RepairFragment;
        }

        private void RepairFragment(string fragmentId)
        {
            /*var memory = _memoryDatabase.GetById(fragmentId);
            if (memory == null)
            {
                Debug.LogWarning($"MemoryRepairService: Memory for fragment '{fragmentId}' not found.");
                return;
            }

            var fragment = memory.fragments.FirstOrDefault(f => f.fragmentId.Equals(fragmentId, StringComparison.OrdinalIgnoreCase));
            if (fragment == null)
            {
                Debug.LogWarning($"MemoryRepairService: No fragment found with ID '{fragmentId}'.");
                return;
            }*/

            if (!_playerProfile.HasRepairedFragment(fragmentId))
            {
                _playerProfile.AddRepairedFragment(fragmentId);
            }
            else
            {
                Debug.Log($"MemoryRepairService: Fragment '{fragmentId}' already repaired.");
            }
        }
    }
}