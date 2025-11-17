using System;
using System.Collections;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Memory.Events;
using _Project.Scripts.Application.Player;
using _Project.Scripts.Data.Memory;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Application.Memory
{
    public enum MemoryVersion
    {
        Corrupted = 0,
        Real = 1
    }
    
    public class MemoryManager
    {
        private string _targetScene;
        private readonly PlayerProfile _playerProfile;
        private AsyncOperation _backgroundLoad;
        private readonly StaticCoroutine _coroutineRunner;
        private readonly MemoryDatabase _memoryDatabase;
        
        // todo: instead of hardcoding, get from config or constants
        private const string MainSceneName = "SecondLayout";
        private GameStateService _gameStateService;

        public MemoryManager()
        {
            MemoryEvents.OnMemoryUnlocked += UnlockMemory;
            MemoryEvents.OnVisitMemory += VisitMemory;
            MemoryEvents.OnMemoryTransitionEnd += HandleTransitionEnd;
            MemoryEvents.OnAllFragmentsCompleted += HandleAllFragmentsCompleted;

            _memoryDatabase = ServiceLocater.GetService<MemoryDatabase>();
            _playerProfile = ServiceLocater.GetService<PlayerProfile>();
            _coroutineRunner = ServiceLocater.GetService<StaticCoroutine>();
            _gameStateService = ServiceLocater.GetService<GameStateService>();
        }

        private void HandleAllFragmentsCompleted()
        {
            _gameStateService.SetState(GameState.Transition);
            MemoryEvents.RaiseMemoryTransitionStart();
            
            _targetScene = MainSceneName;
            _coroutineRunner.StartCoroutine(PreloadScene(_targetScene));
        }

        private void VisitMemory(string memoryId)
        {
            if (string.IsNullOrEmpty(memoryId))
            {
                Debug.LogWarning("MemoryManager: Invalid memoryId for VisitMemory.");
                return;
            }
            
            var sceneName = _memoryDatabase.GetById(memoryId).sceneName;
            if (sceneName == null)
            {
                Debug.LogWarning($"MemoryManager: Memory with ID '{memoryId}' not found in database.");
                return;
            }

            // Unlock if not already unlocked
            UnlockMemory(memoryId);

            _targetScene = sceneName;
            
            _gameStateService.SetState(GameState.Transition);
            MemoryEvents.RaiseMemoryTransitionStart();
            
            _coroutineRunner.StartCoroutine(PreloadScene(_targetScene));
        }
        
        private void UnlockMemory(string memoryId)
        {
            if (!_playerProfile.HasUnlockedMemory(memoryId))
            {
                _playerProfile.AddUnlockedMemory(memoryId);
                
                ToastNotification.Show("New Memory Unlocked! Check your journal.");
            }
        }
        
        
        // todo: move to its own class
        private IEnumerator PreloadScene(string sceneName)
        {
            yield return null;
            _backgroundLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            _backgroundLoad!.allowSceneActivation = false;

            while (_backgroundLoad.progress < 0.9f)
            {
                yield return null;
            }
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