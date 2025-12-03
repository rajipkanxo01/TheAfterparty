using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Project.Scripts.Application.Player
{
    [Serializable]
    public enum ObservationState
    {
        Unknown,
        Verified,
        Contradicted
    }

    [Serializable]
    public class Notes
    {
        public string ObservationId { get; set; }
        public string MemoryId { get; set; }
        public string NoteText { get; set; }
        public ObservationState CurrentState { get; set; } = ObservationState.Unknown;
    }

    public class PlayerProfile
    {
        public string DisplayName { get; private set; }
        public string PlayerId { get; private set; }
        public Sprite Portrait { get; private set; }
        public string MainSceneName { get; private set; }
        
        private readonly HashSet<string> _discoveredClues = new();
        private readonly HashSet<string> _unlockedMemories = new();
        private readonly HashSet<string> _completedFragments = new();
        private readonly HashSet<string> _repairedFragments = new();
        private readonly HashSet<string> _flags = new();
        private readonly Dictionary<string, List<Notes>> _notesByMemory = new();
        private readonly Dictionary<string, HashSet<string>> _selectedContradictionsByMemory = new();

        public IReadOnlyCollection<string> DiscoveredClues => _discoveredClues;
        public IReadOnlyCollection<string> UnlockedMemories => _unlockedMemories;
        public IReadOnlyCollection<string> RepairedFragments => _repairedFragments;
        public IReadOnlyCollection<string> Flags => _flags;

        public IReadOnlyDictionary<string, List<Notes>> AllNotes => _notesByMemory;

        
        public PlayerProfile(string displayName, string playerId, Sprite portrait, string mainSceneName)
        {
            DisplayName = displayName;
            PlayerId = playerId;
            Portrait = portrait;
            MainSceneName = mainSceneName;

            // Always unlocked from the start
            _unlockedMemories.Add("realWorld");
        }

        // --------------------------------------
        // Clues
        // --------------------------------------
        public bool HasDiscoveredClue(string clueId) => _discoveredClues.Contains(clueId);

        public void AddDiscoveredClue(string clueId)
        {
            if (_discoveredClues.Add(clueId))
                Debug.Log($"PlayerProfile: Discovered clue '{clueId}'.");
        }

        // --------------------------------------
        // Memories
        // --------------------------------------
        public bool HasUnlockedMemory(string memoryId) => _unlockedMemories.Contains(memoryId);

        public void AddUnlockedMemory(string memoryId)
        {
            if (_unlockedMemories.Add(memoryId))
                Debug.Log($"PlayerProfile: Unlocked memory '{memoryId}'.");
        }

        // --------------------------------------
        // Notes System
        // --------------------------------------
        public void AddMemoryNotes(string memoryId, List<Notes> notes)
        {
            if (string.IsNullOrEmpty(memoryId) || notes == null || notes.Count == 0)
                return;

            if (!_notesByMemory.TryGetValue(memoryId, out var existingList))
            {
                _notesByMemory[memoryId] = new List<Notes>(notes);
                return;
            }

            foreach (var note in notes)
            {
                if (existingList.Any(n => n.ObservationId == note.ObservationId))
                    continue;

                existingList.Add(note);
            }
        }

        public Notes GetNote(string memoryId, string observationId)
        {
            if (!_notesByMemory.TryGetValue(memoryId, out var notes))
                return null;

            return notes.FirstOrDefault(n => n.ObservationId == observationId);
        }

        public bool SetObservationState(string memoryId, string observationId, ObservationState newState)
        {
            var note = GetNote(memoryId, observationId);

            if (note == null)
            {
                Debug.LogWarning($"PlayerProfile: Cannot update note '{observationId}' in '{memoryId}' (not found).");
                return false;
            }

            if (note.CurrentState == newState)
                return true;

            note.CurrentState = newState;

            Debug.Log($"PlayerProfile: Note '{observationId}' updated to {newState}.");

            // NotesEvents.RaiseNoteStateChanged(memoryId, observationId, newState);
            return true;
        }

        // --------------------------------------
        // Fragment Progress
        // --------------------------------------
        public void AddRepairedFragment(string fragmentId)
        {
            if (_repairedFragments.Add(fragmentId))
                ToastNotification.Show("Fragment repaired successfully! Go back to see the restored memory.");
        }

        public bool HasRepairedFragment(string fragmentId) => _repairedFragments.Contains(fragmentId);

        public void ClearRepairedFragments() => _repairedFragments.Clear();

        public void AddFragmentCompletedMemory(string memoryId)
        {
            if (_completedFragments.Add(memoryId))
                Debug.Log($"PlayerProfile: Memory fragment completed for '{memoryId}'.");
        }

        public bool HasFragmentCompletedMemory(string memoryId) => _completedFragments.Contains(memoryId);

        // --------------------------------------
        // Flags
        // --------------------------------------
        public bool HasFlag(string key) => _flags.Contains(key);

        public void SetFlag(string key, bool enabled)
        {
            if (enabled)
                _flags.Add(key);
            else
                _flags.Remove(key);
        }

        // --------------------------------------
        // Selected Contradictions for Presentation
        // --------------------------------------
        public bool IsContradictionSelected(string memoryId, string observationId)
        {
            if (!_selectedContradictionsByMemory.TryGetValue(memoryId, out var selectedSet))
                return false;

            return selectedSet.Contains(observationId);
        }

        public void ToggleSelectedContradiction(string memoryId, string observationId)
        {
            if (!_selectedContradictionsByMemory.TryGetValue(memoryId, out var selectedSet))
            {
                selectedSet = new HashSet<string>();
                _selectedContradictionsByMemory[memoryId] = selectedSet;
            }

            if (selectedSet.Contains(observationId))
            {
                selectedSet.Remove(observationId);
                Debug.Log($"PlayerProfile: Removed contradiction '{observationId}' from presentation list for '{memoryId}'.");
            }
            else
            {
                selectedSet.Add(observationId);
                Debug.Log($"PlayerProfile: Added contradiction '{observationId}' to presentation list for '{memoryId}'.");
            }
        }

        public int GetSelectedContradictionsCount(string memoryId)
        {
            if (!_selectedContradictionsByMemory.TryGetValue(memoryId, out var selectedSet))
                return 0;

            return selectedSet.Count;
        }

        public void ClearSelectedContradictions(string memoryId)
        {
            if (_selectedContradictionsByMemory.ContainsKey(memoryId))
            {
                _selectedContradictionsByMemory[memoryId].Clear();
                Debug.Log($"PlayerProfile: Cleared all selected contradictions for '{memoryId}'.");
            }
        }
    }
}
