using System;
using System.Collections.Generic;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Player;
using _Project.Scripts.Data.Npc;
using UnityEngine;

namespace _Project.Scripts.Application.Memory
{
    public class MemoryManager
    {
        public event Action<string> MemoryTransitionStarted;
        public event Action<string> MemoryTransitionCompleted;

        private string _targetScene;
        private PlayerProfile _playerProfile;

        public MemoryManager()
        {
            MemoryEvents.OnMemoryUnlocked += UnlockMemory;
            MemoryEvents.OnVisitMemory += VisitMemory;

            _playerProfile = ServiceLocater.GetService<PlayerProfile>();
        }

        private void VisitMemory(string memoryId)
        {
            if (string.IsNullOrEmpty(memoryId))
            {
                Debug.LogWarning("MemoryManager: Invalid memoryId for VisitMemory.");
                return;
            }

            // Unlock if not already unlocked
            UnlockMemory(memoryId);

            _targetScene = memoryId;
            
            

            // Start the transition (fade out / load scene etc.)
            // todo: remove direct completed call and hook into visuals. this is temp solution
            MemoryTransitionCompleted?.Invoke(_targetScene);
        }


        private void UnlockMemory(string memoryId)
        {
            if (!_playerProfile.HasUnlockedMemory(memoryId))
            {
                _playerProfile.AddUnlockedMemory(memoryId);
                ToastNotification.Show("New Memory Unlocked! Visiting Memory.", 2f);
            }
        }

        public void RevisitMemory(string sceneName)
        {
            _targetScene = sceneName;
            MemoryTransitionStarted?.Invoke(_targetScene);
        }

        public void OnVisualsDone()
        {
            MemoryTransitionCompleted?.Invoke(_targetScene);
        }
    }
}