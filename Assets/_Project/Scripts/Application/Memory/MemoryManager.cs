using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Application.Dialogue;
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
            
            // todo: temp dialogue trigger for memory entry. really hacky
            var dialogue = ServiceLocater.GetService<DialogueController>();
            if (dialogue != null)
            {
                dialogue.EnableAutoMode(true);
                dialogue.StartCoroutine(StartMemoryDialogueRoutine());            
            }
        }

        private IEnumerator StartMemoryDialogueRoutine()
        {
            yield return null;                      
            yield return new WaitForSeconds(5f);    

            var dialogue = ServiceLocater.GetService<DialogueController>();
            if (dialogue != null)
            {
                dialogue.EnableAutoMode(true);
                
                GameStateService gameStateService = ServiceLocater.GetService<GameStateService>();
                gameStateService.SetState(GameState.Cutscene);
                
                dialogue.StartDialogue("detective_root", DialogueType.PlayerMonologue);
            }
            else
            {
                Debug.LogWarning("Memory dialogue couldn't start – DialogueController not found.");
            }
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