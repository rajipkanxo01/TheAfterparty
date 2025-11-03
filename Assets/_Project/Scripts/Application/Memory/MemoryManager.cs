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
        public event Action MemoryTransitionStarted;
        public event Action<string> MemoryTransitionCompleted;

        private string _targetScene;
        private PlayerProfile _playerProfile;


        public MemoryManager()
        {
            MemoryEvents.OnMemoryUnlocked += UnlockMemory;

            _playerProfile = ServiceLocater.GetService<PlayerProfile>();
        }

        public void VisitMemory(string memoryId)
        {
            // todo: need to create memory data and memory database in future
            _targetScene = memoryId;
            MemoryTransitionStarted?.Invoke();
        }

        private void UnlockMemory(string memoryId)
        {
            if (!_playerProfile.HasUnlockedMemory(memoryId))
            {
                _playerProfile.AddUnlockedMemory(memoryId);
                ToastNotification.Show("New Memory Unlocked! Check your Memories Journal.", 2f);
            }
        }

        public void RevisitMemory(string sceneName)
        {
            _targetScene = sceneName;
            MemoryTransitionStarted?.Invoke();
        }

        public void OnVisualsDone()
        {
            MemoryTransitionCompleted?.Invoke(_targetScene);
        }
    }
}