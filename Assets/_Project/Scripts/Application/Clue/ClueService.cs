using System.Collections.Generic;
using _Project.Scripts.Application.Core;
using _Project.Scripts.Data.Clues;
using _Project.Scripts.Presentation.Clues;
using UnityEngine;

namespace _Project.Scripts.Application.Clue
{
    public class ClueService
    {
        private readonly ClueManager _clueManager;
        private readonly GameStateService _gameStateService;
        
        private readonly ClueDatabase _clueDatabase;
        private readonly SniffConfig _sniffConfig;
        private float _lastSniffTime;

        public ClueService(ClueManager clueManager, GameStateService gameStateService, SniffConfig sniffConfig)
        {
            _clueManager = clueManager;
            _gameStateService = gameStateService;
            _sniffConfig = sniffConfig;
            
            _clueDatabase = ServiceLocater.GetService<ClueDatabase>();
        }

        public void Examine(string clueId)
        {

            var clueData = _clueManager.GetClueById(clueId);
            if (clueData == null)
            {
                Debug.LogWarning($"ClueService: Clue with ID {clueId} not found.");
            }
            
            ClueEvents.RaiseExamined(clueData);
        }

        public void PerformSniff(Vector3 playerPosition)
        {
            if (!_gameStateService.IsState(GameState.Normal)) return;

            // prevent spamming of sniff
            if (Time.time - _lastSniffTime < _sniffConfig.SniffCooldown)
            {
                ToastNotification.Show("You need to wait before sniffing again.", _sniffConfig.SniffCooldownToastDuration);
                return;
            }

            _lastSniffTime = Time.time;
            var nearbyClues = FindNearbyClues(playerPosition, _sniffConfig.SniffRadius);
            if (nearbyClues.Count == 0)
            {
                ToastNotification.Show("No clues detected nearby.", 1f);
                return;
            }
            
            var clue = nearbyClues[Random.Range(0, nearbyClues.Count)];
            if (!string.IsNullOrEmpty(clue.sniffDialogueNode))
            {
                ClueEvents.RaiseHintFound(clue);
            }
            
        }
        
        private List<ClueData> FindNearbyClues(Vector3 playerPosition, float radius)
        {
            var results = new List<ClueData>();
            var colliders = Physics2D.OverlapCircleAll(playerPosition, radius, LayerMask.GetMask("Clue"));
            
            foreach (var collider in colliders)
            {
                var clueView = collider.GetComponent<ClueObject>();
                if (clueView != null)
                {
                    var clueData = _clueDatabase.GetClueById(clueView.ClueId);
                    if (clueData != null && !results.Contains(clueData))
                    {
                        results.Add(clueData);
                    }
                }
            }

            return results;
        }
    }
}