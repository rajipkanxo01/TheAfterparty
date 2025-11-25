using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Project.Scripts.Application.Player
{
    public class PlayerProfile
    {
        public string DisplayName { get; set; }
        public string PlayerId { get; set; }
        public Sprite Portrait { get; set; }
        public string MainSceneName { get; set; }

        private readonly HashSet<string> _discoveredClues = new();
        private readonly HashSet<string> _unlockedMemories = new();
        private readonly HashSet<string> _repairedFragments = new();
        private readonly HashSet<string> _flags = new();
        private readonly Dictionary<string, List<string>> _allMemoryNotes = new Dictionary<string, List<string>>();
        
        public IReadOnlyCollection<string> GetDiscoveredClues() => _discoveredClues;
        public IReadOnlyCollection<string> GetUnlockedMemories() => _unlockedMemories;
        public IReadOnlyCollection<string> GetRequiredFragments() => _repairedFragments;
        public IReadOnlyDictionary<string, List<string>> GetAllMemoryNotes => _allMemoryNotes;        

        public PlayerProfile(string displayName, string playerId, Sprite portrait, string mainSceneName)
        {
            DisplayName = displayName;
            PlayerId = playerId;
            Portrait = portrait;
            MainSceneName = mainSceneName;

            _unlockedMemories.Add("realWorld");
        }

        public bool HasDiscoveredClue(string clueId)
        {
            return _discoveredClues.Contains(clueId);
        }

        public bool HasUnlockedMemory(string memoryId)
        {
            Debug.Log($"PlayerProfile: Checking unlocked memories for '{memoryId}'. Total unlocked memories: {_unlockedMemories.Count}");
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
        
        public void AddMemoryNotes(string memoryId, List<string> notes)
        {
            if (string.IsNullOrEmpty(memoryId) || notes == null)
                return;

            if (!_allMemoryNotes.TryGetValue(memoryId, out var existingList))
            {
                existingList = new List<string>();
                _allMemoryNotes[memoryId] = existingList;
            }
            
            Debug.Log($"PlayerProfile: Adding memory note '{memoryId}'.");

            foreach (var note in notes.Where(note => !existingList.Contains(note)))
            {
                existingList.Add(note);
            }
        }
        
        public void AddRepairedFragment(string fragmentId)
        {
            if (_repairedFragments.Add(fragmentId))
            {
                ToastNotification.Show("Fragment repaired successfully! Go back to see the restored memory.");
            }
        }
        
        public bool HasRepairedFragment(string fragmentId)
        {
            Debug.Log($"PlayerProfile: Checking repaired fragments for '{fragmentId}'. Total repaired fragments: {_repairedFragments.Count}");
            
            foreach (var repairedFragment in _repairedFragments)
            {
                Debug.Log($"PlayerProfile: Repaired fragment '{repairedFragment}'.");
            }

            return _repairedFragments.Contains(fragmentId);
        }
        
        public bool HasFlag(string key)
        {
            return _flags.Contains(key);
        }

        public void SetFlag(string key, bool value)
        {
            if (value)
            {
                _flags.Add(key);
            }
            else
            {
                _flags.Remove(key);
            }
        }

        
        public void ClearRepairedFragments()
        {
            _repairedFragments.Clear();
        } 
    }
}