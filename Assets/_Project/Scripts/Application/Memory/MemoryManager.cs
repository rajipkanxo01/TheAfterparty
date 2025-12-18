using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Events;
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

        private bool _isLoadingScene = false;
        
        // todo: instead of hardcoding, get from config or constants
        private const string MainSceneName = "SecondLayout";
        private GameStateService _gameStateService;
        private string _currentMemoryId;
        
        public string CurrentMemoryId => _currentMemoryId;
        
        public MemoryManager()
        {
            MemoryEvents.OnMemoryUnlocked += UnlockMemory;
            MemoryEvents.OnVisitMemory += VisitMemory;
            MemoryEvents.OnMemoryTransitionEnd += HandleTransitionEnd;
            MemoryEvents.OnAllFragmentsCompleted += HandleAllFragmentsCompleted;
            
            
            Debug.Log("MemoryManager: Initialized.");

            _memoryDatabase = ServiceLocater.GetService<MemoryDatabase>();
            _playerProfile = ServiceLocater.GetService<PlayerProfile>();
            _coroutineRunner = ServiceLocater.GetService<StaticCoroutine>();
            _gameStateService = ServiceLocater.GetService<GameStateService>();
        }

        private void HandleAllFragmentsCompleted()
        {
            var memory = _memoryDatabase.GetById(_currentMemoryId);
            if (memory == null)
            {
                Debug.LogWarning($"MemoryManager: Cannot add journal entry. Memory '{_currentMemoryId}' not found.");
                return;
            }

            _playerProfile.AddFragmentCompletedMemory(_currentMemoryId);

            var allObservations = memory.memoryObservations; 

            // store in PlayerProfile
            var notesList = new List<Notes>();

            foreach (var memoryObservation in allObservations)
            {
                var notes = new Notes
                {
                    ObservationId = memoryObservation.observationId,
                    MemoryId = _currentMemoryId,
                    NoteText = memoryObservation.observationText,
                    CurrentState = ObservationState.Unknown
                };
                
                notesList.Add(notes);
            }
            
            _playerProfile.AddMemoryNotes(_currentMemoryId, notesList);

            // notify UI
            ToastNotification.Show("New Memory Notes Added! Check your journal.", 5f);
            UIEvents.RaiseJournalNotesChanged();
            // UIEvents.RaiseMemoryNotesAdded(allObservations);
        }

        private void VisitMemory(string memoryId)
        {
            if (_isLoadingScene) return;

            if (string.IsNullOrEmpty(memoryId))
            {
                Debug.LogWarning("MemoryManager: Invalid memoryId for VisitMemory.");
                return;
            }
            
            var memoryData = _memoryDatabase.GetById(memoryId);
            if (memoryData == null)
            {
                Debug.LogWarning($"MemoryManager: Memory with ID '{memoryId}' not found in database.");
                return;
            }
            
            var sceneName = memoryData.sceneName;
            
            // load appropriate scene based on current context
            if (memoryId.Equals("realWorld", StringComparison.InvariantCultureIgnoreCase))
            {
                sceneName = GetRealWorldSceneForCurrentMemory();
            }

            _isLoadingScene = true;

            UnlockMemory(memoryId);
            
            _currentMemoryId = memoryId;
            _targetScene = sceneName;
            
            _gameStateService.SetState(GameState.Transition);
            MemoryEvents.RaiseMemoryTransitionStart();
            
            _coroutineRunner.StartCoroutine(PreloadScene(_targetScene));
        }
        
        private string GetRealWorldSceneForCurrentMemory()
        {
            // Determine which "real world" scene to load based on the current memory's NPC
            if (string.IsNullOrEmpty(_currentMemoryId))
            {
                Debug.LogWarning("MemoryManager: No current memory, defaulting to SecondLayout.");
                return MainSceneName;
            }
            
            var currentMemory = _memoryDatabase.GetById(_currentMemoryId);
            if (currentMemory == null)
            {
                Debug.LogWarning($"MemoryManager: Current memory '{_currentMemoryId}' not found, defaulting to SecondLayout.");
                return MainSceneName;
            }
            
            // Map NPC to their corresponding real world scene
            switch (currentMemory.belongsToNpcId?.ToLower())
            {
                case "detective":
                    return "SecondLayout";
                case "janitor":
                    return "Chapter4-Janitor";
                default:
                    Debug.Log($"MemoryManager: No specific scene mapped for NPC '{currentMemory.belongsToNpcId}', using default.");
                    return MainSceneName;
            }
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

        
        private async void HandleTransitionEnd()
        {
            if (_backgroundLoad == null)
            {
                Debug.LogWarning("MemoryManager: No background load operation to activate.");
                _isLoadingScene = false;
                return;
            }

            Debug.Log($"MemoryManager: Activating scene '{_targetScene}'.");

            _backgroundLoad.allowSceneActivation = true;

            _isLoadingScene = false;
        }


        public void RevisitMemory(string sceneName)
        {
            _targetScene = sceneName;
        }
    }
}