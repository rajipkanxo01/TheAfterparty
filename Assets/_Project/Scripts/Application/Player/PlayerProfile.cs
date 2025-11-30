using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Application.Events;
using _Project.Scripts.Data.Memory;
using UnityEngine;

namespace _Project.Scripts.Application.Player
{
    [Serializable]
    public class ObservationState
    {
        public bool contradicted;
        public bool confirmed;
        public bool evidenceFound;
    }

    public class PlayerProfile
    {
        public string DisplayName { get; set; }
        public string PlayerId { get; set; }
        public Sprite Portrait { get; set; }
        public string MainSceneName { get; set; }

        private readonly HashSet<string> _discoveredClues = new();
        private readonly HashSet<string> _unlockedMemories = new();
        private readonly HashSet<string> _fragmentCompletedMemories = new();
        private readonly HashSet<string> _repairedFragments = new();
        private readonly HashSet<string> _flags = new();
        private readonly Dictionary<string, ObservationState> _observationStates = new();
        private readonly Dictionary<string, List<MemoryObservation>> _allMemoryNotes = new Dictionary<string, List<MemoryObservation>>();
        
        public IReadOnlyCollection<string> GetDiscoveredClues() => _discoveredClues;
        public IReadOnlyCollection<string> GetUnlockedMemories() => _unlockedMemories;
        public IReadOnlyCollection<string> GetRequiredFragments() => _repairedFragments;
        public IReadOnlyDictionary<string, List<MemoryObservation>> GetAllMemoryNotes => _allMemoryNotes;       
        public IReadOnlyCollection<string> GetFlags() => _flags;
        public IReadOnlyCollection<string> GetRepairedFragments() => _repairedFragments;
        public IReadOnlyDictionary<string, ObservationState> GetObservationStates() => _observationStates;

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
        
        public void AddMemoryNotes(string memoryId, List<MemoryObservation> notes)
        {
            if (string.IsNullOrEmpty(memoryId) || notes == null)
                return;

            if (!_allMemoryNotes.TryGetValue(memoryId, out var existingList))
            {
                existingList = new List<MemoryObservation>();
                _allMemoryNotes[memoryId] = existingList;
            }
            
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
        
        public bool IsContradicted(string observationId)
        {
            return _observationStates.TryGetValue(observationId, out var state) && state.contradicted;
        }

        public void SetContradicted(string observationId)
        {
            if (!_observationStates.ContainsKey(observationId))
            {
                _observationStates[observationId] = new ObservationState();
            }

            _observationStates[observationId].contradicted = true;
            UIEvents.RaiseJournalNotesChanged();
        }

        public void AddFragmentCompletedMemory(string currentMemoryId)
        {
            if (_fragmentCompletedMemories.Add(currentMemoryId))
            {
                Debug.Log($"PlayerProfile: Added fragment completed memory '{currentMemoryId}'.");
            }
        }
        
        public bool HasFragmentCompletedMemory(string memoryId)
        {
            return _fragmentCompletedMemories.Contains(memoryId);
        }
        
        public void ClearRepairedFragments()
        {
            _repairedFragments.Clear();
        }

        
    }
}