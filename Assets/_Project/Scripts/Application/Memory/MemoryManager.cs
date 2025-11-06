using System;
using System.Collections;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Application.Memory
{
    public class MemoryManager
    {
        private string _targetScene;
        private readonly PlayerProfile _playerProfile;
        private AsyncOperation _backgroundLoad;
        private readonly StaticCoroutine _coroutineRunner;

        public MemoryManager()
        {
            MemoryEvents.OnMemoryUnlocked += UnlockMemory;
            MemoryEvents.OnVisitMemory += VisitMemory;
            MemoryEvents.OnMemoryTransitionEnd += HandleTransitionEnd;

            _playerProfile = ServiceLocater.GetService<PlayerProfile>();
            _coroutineRunner = ServiceLocater.GetService<StaticCoroutine>();
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
            
            MemoryEvents.RaiseMemoryTransitionStart();
            
            _coroutineRunner.StartCoroutine(PreloadScene(_targetScene));
        }
        
        private void UnlockMemory(string memoryId)
        {
            if (!_playerProfile.HasUnlockedMemory(memoryId))
            {
                _playerProfile.AddUnlockedMemory(memoryId);
                
                // todo: show notification to player
                Debug.Log("MemoryManager: Memory unlocked: " + memoryId);
            }
        }
        
        
        // todo: move to its own class
        private IEnumerator PreloadScene(string sceneName)
        {
            Debug.Log($"MemoryManager: Preloading scene '{sceneName}'...");
            _backgroundLoad = SceneManager.LoadSceneAsync(sceneName);
            _backgroundLoad.allowSceneActivation = false;

            while (_backgroundLoad.progress < 0.9f)
            {
                yield return null; // keep loading
            }

            Debug.Log($"MemoryManager: Scene '{sceneName}' preloaded (waiting for transition end).");
        }
        
        private void HandleTransitionEnd()
        {
            if (_backgroundLoad == null)
            {
                Debug.LogWarning("MemoryManager: No background load operation to activate.");
                return;
            }

            Debug.Log($"MemoryManager: Activating scene '{_targetScene}'.");
            _backgroundLoad.allowSceneActivation = true;
        }

        public void RevisitMemory(string sceneName)
        {
            _targetScene = sceneName;
        }
    }
}