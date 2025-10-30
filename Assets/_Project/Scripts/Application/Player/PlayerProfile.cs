using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Application.Player
{
    public class PlayerProfile
    {
        public string DisplayName { get; set; }
        public string PlayerId { get; set; }
        public Sprite Portrait { get; set; }

        private readonly HashSet<string> _discoveredClues = new();
        private readonly HashSet<string> _unlockedMemories = new();

        public PlayerProfile(string displayName, string playerId, Sprite portrait)
        {
            DisplayName = displayName;
            PlayerId = playerId;
            Portrait = portrait;
        }

        public bool HasDiscoveredClue(string clueId)
        {
            return _discoveredClues.Contains(clueId);
        }

        public bool HasUnlockedMemory(string memoryId)
        {
            return _unlockedMemories.Contains(memoryId);
        }
        
        public void AddDiscoveredClue(string clueId)
        {
            if (_discoveredClues.Add(clueId))
            {
                Debug.Log($"PlayerProfile: Discovered clue '{clueId}'.");
                // maybe trigger some event here to update journal UI
            }
        }

        public void AddUnlockedMemory(string memoryId)
        {
            if (_unlockedMemories.Add(memoryId))
            {
                Debug.Log($"PlayerProfile: Unlocked memory '{memoryId}'.");
                // maybe trigger some event here to update memory UI
            }
        }

        public IReadOnlyCollection<string> GetDiscoveredClues() => _discoveredClues;
        public IReadOnlyCollection<string> GetUnlockedMemories() => _unlockedMemories;
    }
}